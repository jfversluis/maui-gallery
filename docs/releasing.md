# Releasing

The gallery ships to the App Store (iPhone and iPad), the Mac App Store, Google Play and the Microsoft Store. The
[Release workflow](../.github/workflows/release.yml) builds all four, then uploads to each store after you approve it.
Uploads land in a testing stage (TestFlight, Play's internal track, a Partner Center draft); you promote them to the
public in each store's console. Store texts and answers live in [store-listing.md](store-listing.md).

- [Release a version](#release-a-version): the steps for every release.
- [One-time setup](#one-time-setup): environments, keys and app records, per store.
- [Store readiness](#store-readiness): what each store needs before the first submission.

## Release a version

1. Bump `Version` in [`Directory.Build.props`](../Directory.Build.props) (major.minor.patch, no suffix). Update the
   release notes in [`eng/release/whatsnew/whatsnew-en-US`](../eng/release/whatsnew/whatsnew-en-US) (Google Play,
   500 characters max), [`whats_new.txt`](../eng/release/app-store/metadata/en-US) for the App Store (from the second
   version on) and "What's new" in [store-listing.md](store-listing.md). Merge to `main`.
2. Tag the merge commit and push the tag:

   ```bash
   git tag v1.1.0 && git push origin v1.1.0
   ```

   The workflow fails if the tag doesn't match `Version`.
3. Open the run (Actions > Release). The **Release plan** job lists which stores will get the build and why others
   are skipped. When the builds finish, click **Review deployments**, select the stores and approve. Check that each
   upload job succeeded before you promote.
4. Promote the build in each store:

   | Store | The workflow leaves the build in | To release it |
   | --- | --- | --- |
   | App Store | TestFlight (iOS) | App Store Connect > the app > iOS: add the version, pick the build, fill in "What's New", **Add for Review**, then **Submit for Review** |
   | Mac App Store | TestFlight (macOS) | The same on the macOS platform |
   | Google Play | Internal testing | Play Console > Test and release > Internal testing > **Promote release** > Production, then review and roll out (updates can use a staged rollout) |
   | Microsoft Store | A draft submission | Partner Center > the app > the pending submission: check it, then **Submit to the Store** |

   Changed the App Store screenshots or texts? Run the [App Store listing](#apple-screenshots-and-texts-optional)
   workflow and check the result **before Add for Review**: App Store Connect locks them after that.

5. The workflow creates a GitHub release for the tag with the packages it built and generated notes.

### Other runs

Start the workflow by hand with **Actions > Release > Run workflow**, or with `gh workflow run release.yml`:

| Input | Default | Use |
| --- | --- | --- |
| `stores` | `none` | `none` only builds. `all` or one store also uploads, after approval. |
| `build_number` | the run number | Set it to skip past a build number used by an upload from elsewhere. |
| `google_play_track` | `internal` | `alpha` (closed testing), `beta` (open testing) or `production` |
| `google_play_status` | `completed` | `draft` creates the release without rolling it out (needed until the app has been published once) |
| `microsoft_store_mode` | `draft` | `publish` submits for certification; `flight` submits to the package flight in `MICROSOFT_STORE_FLIGHT_ID` |

For example, to upload a new build of the current version to TestFlight only:
`gh workflow run release.yml --ref v1.1.0 -f stores=app-store`. Tag runs use the defaults, or the repository
variables `GOOGLE_PLAY_TRACK`, `GOOGLE_PLAY_RELEASE_STATUS` and `MICROSOFT_STORE_MODE` when set; manual runs use
their inputs.

### When an upload fails

Read the job's error first, and check the store's console: the upload may have arrived anyway.

- **Nothing arrived** (a credential or network error): fix the cause and use **Re-run failed jobs**. The rerun uses
  the same packages and build number.
- **The store already has this build number** (or rejected the build after processing): start a new run for that
  store only, which gets a new run number: `gh workflow run release.yml --ref v1.1.0 -f stores=google-play`.
- **Microsoft Store**: a new submission needs a higher package version, so bump `Version` and tag again.

## Versions

| What | Where | Example |
| --- | --- | --- |
| Version people see (`ApplicationDisplayVersion`, `CFBundleShortVersionString`, `versionName`) | `Version` in [`Directory.Build.props`](../Directory.Build.props) | `1.0.0` |
| Build number (`ApplicationVersion`, `CFBundleVersion`, `versionCode`) | The Release workflow's run number, or its `build_number` input | `42` |
| MSIX package version | `Version` plus `.0`: the Store reserves the last part | `1.0.0.0` |

Local builds use build number 1; override it with `-p:ApplicationVersion=<number>`.

- **Apple and Google** need a higher build number for every upload, even of the same version. The run number only
  grows, so that happens on its own. Google Play's `versionCode` can never go down and must stay under 2,100,000,000.
- **Microsoft Store** compares package versions, so every Store submission needs a new `Version`.

## What the workflow does

| Job | Runs on | Does | Environment |
| --- | --- | --- | --- |
| Version | `ubuntu-latest` | Reads `Version`, picks the build number, checks the tag | |
| Release plan | `ubuntu-latest` | Decides which uploads run; lists the reasons in the run summary | |
| Android (AAB) | `ubuntu-latest` | `android-<version>-<build>`: `com.jfversluis.mauigallery-Signed.aab`, signed with the upload key | |
| iOS (IPA) | `xcode-27` | `ios-<version>-<build>`: `MauiGallery.ipa`, signed for the App Store | |
| Mac Catalyst (PKG) | `xcode-27` | `maccatalyst-<version>-<build>`: `MauiGallery-<version>.pkg`, universal (Apple silicon and Intel), signed for the Mac App Store | |
| Windows (MSIX) | `windows-latest` | `windows-<version>-<build>-win-x64` and `-win-arm64`: one unsigned `.msix` each | |
| Windows (MSIX bundle) | `windows-latest` | `windows-<version>-<build>-msixbundle`: both packages in one `.msixbundle`, which the Store signs | |
| App Store Connect (iOS) / (Mac) | `xcode-27` | Uploads the IPA / PKG with `xcrun altool --upload-package` and waits for processing | `app-store` |
| Google Play | `ubuntu-latest` | Uploads the AAB with [r0adkll/upload-google-play](https://github.com/r0adkll/upload-google-play) and the release notes | `google-play` |
| Microsoft Store | `windows-latest` | Submits the bundle with the [Microsoft Store Developer CLI](https://learn.microsoft.com/windows/apps/publish/msstore-dev-cli/overview) (`msstore publish`) | `microsoft-store` |
| GitHub release | `ubuntu-latest` | Tag pushes only: creates the release with the four packages and generated notes | |

Build jobs sign their package when the signing secrets exist. Without them they still build, warn, and produce a
package the store won't take (a debug-signed AAB, unsigned IPA and PKG, an MSIX with a placeholder identity), so the
workflow works on forks and before setup. An upload runs only when its store is configured **and** its package is
signed; otherwise the plan job skips it with a notice. If an approved upload job finds its environment secrets
missing, it fails with the names of the missing secrets. Uploads to the same store never overlap: a second one waits
for the first.

Third-party actions are pinned to commit SHAs. The scripts the jobs run are in [`eng/release`](../eng/release).

## Build configuration

Release builds use the best pipeline .NET 11 supports **for this app** on each platform. Debug builds keep the defaults
(fast builds, the DevFlow agent). The settings are in [`MauiGallery.csproj`](../src/MauiGallery/MauiGallery.csproj);
`dotnet publish` in the Release workflow needs no extra arguments for them.

| Platform | Release pipeline | Trimming | NativeAOT |
| --- | --- | --- | --- |
| iOS | CoreCLR, composite full ReadyToRun, interpreter for the rest (the .NET 11 default) | Partial (SDK default) | Supported by .NET 11, not used: BlazorWebView fails |
| Mac Catalyst | Same as iOS, universal (arm64 and x64) | Partial (SDK default) | Supported by .NET 11, not used: BlazorWebView fails |
| Android | CoreCLR, composite partial ReadyToRun (the .NET 11 default), **R8** for the Java code | Partial (SDK default) | Experimental in .NET 11 ([XA1040](https://learn.microsoft.com/dotnet/android/messages/xa1040)), not used |
| Windows | CoreCLR, ReadyToRun (the default) | None (default) | Builds and runs unpackaged; the MSIX build fails ([dotnet/maui#35641](https://github.com/dotnet/maui/issues/35641)) and BlazorWebView fails |

Why not NativeAOT or full trimming: the BlazorWebView sample. Razor components and their JS interop aren't compatible with
either in .NET 11 ([dotnet/aspnetcore#70364](https://github.com/dotnet/aspnetcore/issues/70364)). A NativeAOT build shows
"An unhandled error has occurred" in the component (its IPC uses reflection-based System.Text.Json), and full trimming
strips the components, which the app only references by type. Everything else in the app works with NativeAOT. Once
Blazor supports it, enable it for iOS and Mac Catalyst with `<PublishAot>true</PublishAot>` (not conditioned on the
configuration: it also selects feature switches) and check the trim and AOT warnings of `dotnet publish`.

What the app uses:

| Feature | Trimming and NativeAOT |
| --- | --- |
| XAML | Source-generated with compiled bindings (`x:DataType` everywhere, `MauiStrictXamlCompilation`). `{OnIdiom}` compiles to `OnIdiom<T>` from .NET MAUI 11.0.10 on, so `MauiVersion` is pinned to it in `Directory.Build.props`. `RelativeSource Self` and `TemplatedParent` bindings aren't compiled yet, so those few are set in code. On Windows, `{OnPlatform}` isn't resolved at compile time. |
| HybridWebView | Safe: the sample passes a source-generated `JsonSerializerContext` to `SetInvokeJavaScriptTarget` and serializes its messages with it. |
| BlazorWebView | Not compatible with NativeAOT or full trimming (above); works with the default partial trimming. |
| Maps, CommunityToolkit.Mvvm, IconFont.Maui.FluentIcons | No trim or AOT warnings. |

The trim, AOT and single-file analyzers run in every build on iOS, Mac Catalyst and Android, and warnings fail the build,
so the app's own code stays trim- and AOT-safe. (.NET MAUI turns the trim analyzer off when an app doesn't use full
trimming; the project turns it back on. Not on Windows, because of `{OnPlatform}`.)

R8 shrinks the Java code (`classes*.dex` went from 18.5 MB to 5.8 MB). It writes its mapping file to
`bin/Release/net11.0-android/publish/mapping.txt` and also embeds it in the AAB
(`BUNDLE-METADATA/com.android.tools.build.obfuscation/proguard.map`), so Google Play deobfuscates Java stack traces
without a separate upload. No custom ProGuard rules are needed: .NET for Android generates keep rules for the Java types
.NET code uses, and the AndroidX and Google Play services libraries bring their own. R8 problems only show at runtime:
after changing Java dependencies or Android build settings, run the
[Android Release smoke test](../.github/workflows/android-release-smoke.yml) workflow (Actions > Run workflow; it must be on the default branch), which
starts the Release APK on an emulator, opens representative pages, clicks inside the BlazorWebView and checks logcat.

Measured on Nov 27, 2026 (Apple silicon Mac under heavy load, so build times are rough; Android startup on the hosted
Android emulator in CI, x86_64, median of 10 cold starts with `am start -W`; iOS startup on the iPhone 18 Pro Max
simulator, from launch to the Home page, median of 8; Windows on `windows-latest`, median of 5):

| | Before (MAUI 11.0.0, defaults) | Now | NativeAOT (not shipped) |
| --- | --- | --- | --- |
| iOS IPA | 30.7 MB | 29.6 MB | 27.0 MB |
| iOS startup (simulator) | 4.8 s | 4.6 s | 3.5 s |
| Android AAB | 39.8 MB | 37.4 MB | |
| Android download, arm64 (bundletool) | 24.6 MB | 20.0 MB | |
| Android startup (emulator) | 2.1 s | 2.1 s | |
| Mac Catalyst PKG (universal) | 51.7 MB | 49.5 MB | |
| Mac Catalyst app (universal) | 267 MB | 252 MB | |
| Windows startup to main window (hosted runner, unpackaged) | | 1.1 s | 0.5 s |
| Build time, IPA / AAB / PKG | 110 / 133 / 173 s | 63 / 130 / 118 s | 84 s (IPA) |

Sources: [Runtimes and compilation](https://learn.microsoft.com/dotnet/maui/deployment/runtimes-compilation?view=net-maui-11.0),
[NativeAOT deployment](https://learn.microsoft.com/dotnet/maui/deployment/nativeaot?view=net-maui-11.0),
[Trimming](https://learn.microsoft.com/dotnet/maui/deployment/trimming?view=net-maui-11.0),
[Mono to CoreCLR on mobile](https://learn.microsoft.com/dotnet/core/compatibility/maui/11/mobile-code-generation),
[What's new in .NET MAUI 11](https://learn.microsoft.com/dotnet/maui/whats-new/dotnet-11?view=net-maui-11.0),
[Android build properties (`AndroidLinkTool`)](https://learn.microsoft.com/dotnet/android/building-apps/build-properties#androidlinktool),
[dotnet/maui#38661](https://github.com/dotnet/maui/pull/38661) (`{OnIdiom}`).

## One-time setup

Run the commands from a clone of the repo with the [GitHub CLI](https://cli.github.com) signed in as an admin.
Replace the example values with yours. `gh secret set NAME` without `--body` prompts for the value, so it doesn't end
up in your shell history. `base64 -i` is the macOS syntax; on Linux use `base64 -w0 <file>`.

### GitHub environments

Each upload job runs in an environment that needs your approval, and only `v*` tags and `main` may deploy to it:

```bash
repo=jfversluis/maui-gallery
me=$(gh api user --jq .id)
for env in app-store google-play microsoft-store; do
  gh api -X PUT "repos/$repo/environments/$env" --input - <<EOF
{"reviewers":[{"type":"User","id":$me}],"prevent_self_review":false,
 "deployment_branch_policy":{"protected_branches":false,"custom_branch_policies":true}}
EOF
  gh api -X POST "repos/$repo/environments/$env/deployment-branch-policies" -f name='v*' -f type=tag
  gh api -X POST "repos/$repo/environments/$env/deployment-branch-policies" -f name=main -f type=branch
done
```

Keep `prevent_self_review` off while you're the only reviewer, or nobody can approve.

### Apple: signing (iOS and Mac)

These repository secrets let the build jobs sign. Set them once, and again when a certificate or profile expires
(certificates last a year).

| Secret | What it is |
| --- | --- |
| `APPLE_CERTIFICATE_P12_BASE64` | The **Apple Distribution** certificate with its private key, exported as `.p12`, base64 encoded. Signs the iOS and the Mac app |
| `APPLE_CERTIFICATE_PASSWORD` | The `.p12` password |
| `APPLE_INSTALLER_CERTIFICATE_P12_BASE64` | The **Mac Installer Distribution** certificate (shown as "3rd Party Mac Developer Installer") as `.p12`, base64 encoded. Signs the `.pkg` |
| `APPLE_INSTALLER_CERTIFICATE_PASSWORD` | Optional: its password, if it differs from `APPLE_CERTIFICATE_PASSWORD` |
| `APPLE_PROVISIONING_PROFILE_IOS_BASE64` | An **App Store Connect** distribution profile for iOS, App ID `com.jfversluis.mauigallery`, base64 encoded |
| `APPLE_PROVISIONING_PROFILE_MAC_BASE64` | A **Mac App Store Connect** distribution profile for Mac Catalyst, the same App ID, base64 encoded |

1. In [Certificates, Identifiers & Profiles](https://developer.apple.com/account/resources), make sure the App ID
   `com.jfversluis.mauigallery` exists. It's a universal App ID (iOS, macOS and more), so the Mac Catalyst app
   shares it and no Mac Catalyst setting is needed.
2. Create the two certificates in Xcode (Settings > Accounts > Manage Certificates > + > Apple Distribution, then
   Mac Installer Distribution) and export each from Keychain Access as `.p12` with a password.
3. Create two distribution profiles for the App ID: **App Store Connect** (iOS) and **Mac App Store Connect**
   (macOS, type Mac Catalyst), and download them.
4. Store them:

   ```bash
   base64 -i distribution.p12 | gh secret set APPLE_CERTIFICATE_P12_BASE64
   gh secret set APPLE_CERTIFICATE_PASSWORD
   base64 -i installer.p12 | gh secret set APPLE_INSTALLER_CERTIFICATE_P12_BASE64
   base64 -i MauiGallery_iOS_AppStore.mobileprovision | gh secret set APPLE_PROVISIONING_PROFILE_IOS_BASE64
   base64 -i MauiGallery_Mac_AppStore.provisionprofile | gh secret set APPLE_PROVISIONING_PROFILE_MAC_BASE64
   ```

All of these are set for this repository (November 2026). The maintainer keeps the originals, with their passwords
and expiry dates, outside the repository.

To sign on a Mac instead, with the certificates in the login keychain and the profiles installed (double-click
them), pass the identity by its SHA-1 when the keychain has more than one Apple Distribution certificate:

```bash
dotnet publish src/MauiGallery -f net11.0-ios -c Release -p:RuntimeIdentifier=ios-arm64 -p:ApplicationVersion=<build> \
  -p:CodesignKey=<SHA-1> -p:CodesignProvision="MAUI Gallery iOS App Store"
dotnet publish src/MauiGallery -f net11.0-maccatalyst -c Release -p:ApplicationVersion=<build> -p:CreatePackage=true \
  -p:EnableCodeSigning=true -p:EnablePackageSigning=true -p:CodesignKey=<SHA-1> \
  -p:CodesignProvision="MAUI Gallery Mac Catalyst App Store" \
  -p:PackageSigningKey="3rd Party Mac Developer Installer: Gerald Versluis (AY5KBJ6RN9)"
```

Use the build number of a Release workflow run (or higher), so later workflow builds still count up.

The team ID defaults to `AY5KBJ6RN9`; set the `APPLE_TEAM_ID` variable to use another team. Signing with a profile
switches the Mac app to `Platforms/MacCatalyst/Entitlements.AppStore.plist` (the sandbox plus the keychain access
group SecureStorage needs). Unsigned and local Release builds use `Entitlements.plist`, and Debug builds
`Entitlements.Debug.plist`, which adds the network server entitlement for DevFlow.

### Apple: App Store Connect uploads

1. The app record exists: ".NET MAUI Gallery", Apple ID **6839980440**, SKU `mauigallery`, bundle ID
   `com.jfversluis.mauigallery`, iOS and macOS in one record, free in all regions, released manually after approval.
   The workflows use that Apple ID; set the `APP_STORE_APP_ID` variable if it ever changes.
2. Create an API key: Users and Access > Integrations > App Store Connect API > Team Keys > **+**, access
   **App Manager**. (The first time, the Account Holder has to click **Request Access** there.) Download
   `AuthKey_<key id>.p8`, which is only possible once, and note the key ID and the issuer ID shown above the list.
3. Store them, then delete the downloaded key or keep it in a password manager:

   ```bash
   gh variable set APP_STORE_CONNECT_KEY_ID --body 'ABC123DEFG'      # your key ID
   gh variable set APP_STORE_CONNECT_ISSUER_ID --body '69a6de7f-...'  # your issuer ID
   gh secret set APP_STORE_CONNECT_PRIVATE_KEY --env app-store < ~/Downloads/AuthKey_ABC123DEFG.p8
   ```

4. In TestFlight, create an internal testing group with yourself in it and turn on **automatic distribution**, so
   new builds reach your devices without adding them by hand.

The upload uses `xcrun altool --upload-package` from Xcode 27, Apple's command-line uploader; it finds the app by
the bundle ID. App Store apps don't need notarization. Export compliance is answered in `Info.plist` (`ITSAppUsesNonExemptEncryption` is false), so
TestFlight builds are ready to test as soon as processing finishes.

### Apple: screenshots and texts (optional)

The [App Store listing workflow](../.github/workflows/app-store-listing.yml) uploads the screenshots and texts in
[`eng/release/app-store`](../eng/release/app-store) to the iOS and macOS versions being prepared, with the same API key
and the same `app-store` approval. It replaces what's in App Store Connect, so it only runs by hand:

```bash
gh workflow run app-store-listing.yml                      # screenshots only
gh workflow run app-store-listing.yml -f metadata=true     # screenshots and texts
```

| Folder | Goes to |
| --- | --- |
| `screenshots/en-US/iphone-6.9`, `ipad-13`, `mac` | The 6.9" iPhone, 13" iPad and Mac screenshots, in file name order (10 at most) |
| `metadata/en-US/*.txt` | Name, subtitle and privacy policy URL (app information); description, keywords, promotional text, support and marketing URLs, copyright and, from the second version on, `whats_new.txt` (both platforms) |

Pushes that change these files only validate them (sizes, no alpha channel, text limits);
`python3 eng/release/app-store-listing.py --check --screenshots --metadata` does the same locally. Without the
App Store Connect variables the upload is skipped. Each device's screenshots are deleted and uploaded again, so if a
run fails halfway, run it again. The screenshots go through the screenshot set API, which Apple has deprecated in favor
of the App Asset Library but still serves.

### Google Play

1. **Signing.** Create an upload key once and keep a backup outside GitHub; Play App Signing holds the app signing
   key, and this key only proves that uploads come from you.

   ```bash
   keytool -genkeypair -v -keystore upload.keystore -alias upload -keyalg RSA -keysize 2048 -validity 10000
   base64 -i upload.keystore | gh secret set ANDROID_KEYSTORE_BASE64
   gh secret set ANDROID_KEYSTORE_PASSWORD
   gh secret set ANDROID_KEY_ALIAS --body upload
   gh secret set ANDROID_KEY_PASSWORD
   ```

2. **Maps key.** Create the key as described in [Map keys and costs](#map-keys-and-costs) and store it with
   `gh secret set MAPS_API_KEY` before the first release, or the Map sample shows a "needs an API key" message.
3. **App.** In the [Play Console](https://play.google.com/console), create the app (".NET MAUI Gallery", app, free)
   and complete the [store readiness](#store-readiness) items. Upload the first bundle by hand: run the workflow,
   download the `android-*` artifact, and create an internal testing release with it. That registers your upload key
   with Play App Signing; the API can't create the app or its first release.
4. **Service account.** In the [Google Cloud console](https://console.cloud.google.com), pick or create a project,
   enable the **Google Play Android Developer API**, and create a service account with no roles (IAM & Admin >
   Service accounts). Under Keys, add a JSON key. In the Play Console, open Users and permissions > **Invite new
   users**, enter the service account's email, and under App permissions add the app with **Release apps to testing
   tracks** (add **Release to production, exclude devices, and use Play App Signing** if you want the workflow to
   upload to production).
5. Store it, then delete the JSON file:

   ```bash
   gh variable set GOOGLE_PLAY_SERVICE_ACCOUNT --body 'play-upload@my-project.iam.gserviceaccount.com'
   gh secret set GOOGLE_PLAY_SERVICE_ACCOUNT_JSON --env google-play < ~/Downloads/my-project-0123456789ab.json
   ```

If your Cloud organization blocks service account keys, use
[workload identity federation](https://github.com/google-github-actions/auth#workload-identity-federation-through-a-service-account)
instead: allow this repository to impersonate the service account, set the `GOOGLE_PLAY_WORKLOAD_IDENTITY_PROVIDER`
variable to the provider's full name, and skip the JSON secret.

Until the app has been published, the Play API only accepts draft releases. If an upload fails with "Only releases
with status draft may be created on draft app", run `gh variable set GOOGLE_PLAY_RELEASE_STATUS --body draft` (or
pick `draft` in a manual run), roll the release out by hand, and delete the variable once the app is live.

Personal developer accounts created after 13 November 2023 also need a closed test with at least 12 testers for
14 days before they can publish to production
([Play Console Help](https://support.google.com/googleplay/android-developer/answer/14151465)).

### Microsoft Store

1. **Identity.** In [Partner Center](https://partner.microsoft.com/dashboard), reserve the app name and open Product
   management > Product identity. Set the three values shown there as variables (they aren't secrets):

   ```bash
   gh variable set WINDOWS_IDENTITY_NAME --body '12345Publisher.NETMAUIGallery'  # Package/Identity/Name
   gh variable set WINDOWS_PUBLISHER --body 'CN=00000000-0000-0000-0000-000000000000'  # Package/Identity/Publisher
   gh variable set WINDOWS_PUBLISHER_DISPLAY_NAME --body 'Gerald Versluis'         # Package/Properties/PublisherDisplayName
   ```

2. **First submission, by hand.** Run the workflow, download the `windows-*-msixbundle` artifact, create the first
   submission in Partner Center with it (see [store readiness](#store-readiness)), submit it, and wait until the app
   is live: the Store Developer CLI only updates apps that are already in the Store.
3. **API access.** If your Partner Center account has no Microsoft Entra tenant yet, associate one (Account settings
   > Tenants). Then, under Account settings > User management > Microsoft Entra applications, create (or add) an
   application with the **Manager** role and add a key. Note the tenant ID, client ID and key. The seller ID is
   under Account settings > Legal info > Developer. The product ID (Store ID, like `9N...`) is on the app's Product
   identity page.
4. Store them:

   ```bash
   gh variable set MICROSOFT_STORE_PRODUCT_ID --body '9NXXXXXXXXXX'   # the Store ID
   gh secret set MICROSOFT_STORE_TENANT_ID --env microsoft-store
   gh secret set MICROSOFT_STORE_CLIENT_ID --env microsoft-store
   gh secret set MICROSOFT_STORE_CLIENT_SECRET --env microsoft-store
   gh secret set MICROSOFT_STORE_SELLER_ID --env microsoft-store
   ```

Entra keys expire (24 months at most): create a new one and update `MICROSOFT_STORE_CLIENT_SECRET` in time.

In every mode the CLI replaces a pending submission with a new one based on the live listing, so submit or delete
any listing edits in Partner Center before you approve the upload. `draft` (the default) leaves the new submission
for you to check and submit, `publish` submits it for certification straight away, and `flight` submits to the
[package flight](https://learn.microsoft.com/windows/apps/publish/package-flights) in the `MICROSOFT_STORE_FLIGHT_ID`
variable.

The Map sample ships without an Azure Maps key: on Windows it asks people to paste their own and keeps it in memory
only. To ship one anyway, add an `AZURE_MAPS_KEY` secret (see [Map keys and costs](#map-keys-and-costs)).

### Everything the workflow reads

| Name | Kind | Where | Needed for |
| --- | --- | --- | --- |
| `APPLE_CERTIFICATE_P12_BASE64`, `APPLE_CERTIFICATE_PASSWORD` | Secret | Repository | Signing iOS and Mac |
| `APPLE_INSTALLER_CERTIFICATE_P12_BASE64` (`_PASSWORD` optional) | Secret | Repository | Signing the Mac `.pkg` |
| `APPLE_PROVISIONING_PROFILE_IOS_BASE64`, `APPLE_PROVISIONING_PROFILE_MAC_BASE64` | Secret | Repository | Signing iOS / Mac |
| `APPLE_TEAM_ID` | Variable (optional) | Repository | Another Apple team than `AY5KBJ6RN9` |
| `APP_STORE_CONNECT_KEY_ID`, `APP_STORE_CONNECT_ISSUER_ID` | Variable | Repository | Turns on App Store Connect uploads |
| `APP_STORE_CONNECT_PRIVATE_KEY` | Secret | `app-store` | App Store Connect uploads and the listing workflow |
| `APP_STORE_APP_ID` | Variable (optional) | Repository | Another App Store Connect app than `6839980440` |
| `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASSWORD`, `ANDROID_KEY_ALIAS`, `ANDROID_KEY_PASSWORD` | Secret | Repository | Signing Android |
| `MAPS_API_KEY` | Secret | Repository | The Map sample on Android (also CI) |
| `GOOGLE_PLAY_SERVICE_ACCOUNT` | Variable | Repository | Turns on Google Play uploads |
| `GOOGLE_PLAY_SERVICE_ACCOUNT_JSON` | Secret | `google-play` | Google Play uploads (or `GOOGLE_PLAY_WORKLOAD_IDENTITY_PROVIDER`) |
| `GOOGLE_PLAY_TRACK`, `GOOGLE_PLAY_RELEASE_STATUS` | Variable (optional) | Repository | Tag runs: track (default `internal`) and status (default `completed`) |
| `WINDOWS_IDENTITY_NAME`, `WINDOWS_PUBLISHER`, `WINDOWS_PUBLISHER_DISPLAY_NAME` | Variable | Repository | The MSIX identity the Store accepts |
| `AZURE_MAPS_KEY` | Secret (optional) | Repository | The Map sample on Windows (also CI) |
| `MICROSOFT_STORE_PRODUCT_ID` | Variable | Repository | Turns on Microsoft Store submissions |
| `MICROSOFT_STORE_TENANT_ID`, `MICROSOFT_STORE_CLIENT_ID`, `MICROSOFT_STORE_CLIENT_SECRET`, `MICROSOFT_STORE_SELLER_ID` | Secret | `microsoft-store` | Microsoft Store submissions |
| `MICROSOFT_STORE_MODE`, `MICROSOFT_STORE_FLIGHT_ID` | Variable (optional) | Repository | Tag runs: `draft` (default), `flight` or `publish` |

## Store readiness

Do these once per store before the first submission; texts and answers are in [store-listing.md](store-listing.md).

| Item | App Store / Mac App Store | Google Play | Microsoft Store |
| --- | --- | --- | --- |
| Listing texts | Done in App Store Connect (also in `eng/release/app-store/metadata`) | Short and full description | Reuse the Google Play texts |
| Category | Developer Tools (secondary: Education); the Mac app sets it in `Info.plist` | Tools | Developer tools |
| Screenshots | iPhone 6.9" (1320×2868), iPad 13" (2064×2752), Mac (2880×1800): in `eng/release/app-store/screenshots`, **to upload** (listing workflow) | Phone (1080×2424): captured, **to upload** | **Missing**: at least one desktop PNG of 1366×768 or larger; 4 to 8 at 1920×1080 or 3840×2160 recommended |
| Other art | None (the icon comes from the build) | Icon 512×512 and feature graphic 1024×500: **to make** | Optional but recommended: 1:1 app tile icon 300×300; 2:3 poster 720×1080 and 1:1 box art 1080×1080 |
| Privacy | Privacy policy URL (in the texts), App Privacy: data not collected, **to do** | Privacy policy URL, data safety: **decide** (see below) | Privacy policy URL |
| Age rating | Questionnaire, including the **unrestricted web access** decision ([store-listing.md](store-listing.md#age-ratings)), **to do** | IARC questionnaire (Everyone / PEGI 3) | IARC questionnaire |
| Other | Review notes (no account; permissions only when a sample is used) | App access (no login), ads (none), target audience 13+, government app (no) | Declare the location, microphone and webcam capabilities |

- The privacy policy URL, `https://github.com/jfversluis/maui-gallery/blob/main/docs/privacy.md`, only works once
  [privacy.md](privacy.md) is on `main`.
- Store screenshots are captured with DevFlow from the running app, without device frames (see
  [store-listing.md](store-listing.md#screenshots)). The App Store and Mac sets are in the repo for the listing
  workflow; upload the Google Play set by hand. Capture the Windows ones on a Windows machine at 1920×1080 (or
  download them from the [Windows UI workflow](windows-validation.md) at the expanded width).
- Set `MAPS_API_KEY` before building the release (Google Play), or the Map sample can't show a map on Android.
- Google Play data safety: the Maps SDK for Android in the Map sample collects
  [request metadata, an SDK identifier, the IP address and crash data](https://developers.google.com/maps/documentation/android-sdk/play-data-disclosure).
  Decide with that page whether the form (and [privacy.md](privacy.md)) must declare it before submitting.
- App Store Connect still needs: content rights (no third-party content), the App Review contact details and, to
  distribute in the EU, the Digital Services Act trader status (App Store Connect > Business). The release option is
  set to manual.

## Map keys and costs

Any key that ships inside an app can be read out of the package, so treat both map keys as public and limit what they
can do rather than relying on keeping them secret.

- **Google Maps (Android, `MAPS_API_KEY`).** The Maps SDK for Android has unlimited free map loads when no map ID is
  used, which is how .NET MAUI's `Map` loads it. Google still requires a billing account on the project. Restrict the
  key to Android apps with the package name `com.jfversluis.mauigallery` and the SHA-1 of the **app signing**
  certificate (Play Console > Test and release > App integrity), plus your upload key's SHA-1 for testing, **and** to
  the Maps SDK for Android API, so a copied key can't call paid APIs. Add a budget alert in Cloud Billing as a safety
  net; budgets alert but don't stop spending.
- **Azure Maps (Windows, `AZURE_MAPS_KEY`, optional).** Store builds leave it out: the Map page lets people paste their
  own key. If you do ship one, create an Azure Maps account (Gen2 pricing) and copy its primary key
  (Authentication). Base map tiles have a free monthly allowance (5,000 transactions of up to 15 tiles each), then
  they're billed per 1,000 transactions; traffic has a smaller allowance. To rule out charges, put the account in a
  subscription with a spending limit, such as a Visual Studio subscription's monthly Azure credit: Azure disables its
  resources when the credit runs out instead of billing you. Otherwise, add a budget alert in Cost Management.
