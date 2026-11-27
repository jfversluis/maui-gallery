# Releasing

The gallery ships to the App Store (iPhone and iPad), the Mac App Store, Google Play and the Microsoft Store. The
[Release workflow](../.github/workflows/release.yml) builds the store packages; uploading and submitting them is a
manual step, described in the checklists below. The store texts live in [store-listing.md](store-listing.md).

## Versions

| What | Where | Example |
| --- | --- | --- |
| Version people see (`ApplicationDisplayVersion`, `CFBundleShortVersionString`, `versionName`) | `Version` in [`Directory.Build.props`](../Directory.Build.props) | `1.0.0` |
| Build number (`ApplicationVersion`, `CFBundleVersion`, `versionCode`) | The Release workflow's run number, or its `build_number` input | `42` |

Local builds use build number 1. Override it with `-p:ApplicationVersion=<number>`.

Every upload needs a higher build number than the last one in that store, even for the same version. The run
number only grows, so this happens on its own; set `build_number` by hand only to skip past a number used by an
upload from elsewhere (Google Play's `versionCode` can never go down, and must stay under 2,100,000,000).

To release a version:

1. Bump `Version` in `Directory.Build.props` (major.minor.patch, no suffix) and update "What's new" in
   [store-listing.md](store-listing.md). Commit.
2. Tag the commit and push the tag: `git tag v1.1.0 && git push origin v1.1.0`. The workflow fails if the tag
   doesn't match `Version`.
3. Download the artifacts from the workflow run and follow the store checklists below.

You can also start the workflow by hand (Actions > Release > Run workflow) to build packages from any branch.

## What the workflow builds

| Job | Runner | Output artifact | Signed with |
| --- | --- | --- | --- |
| Android (AAB) | `ubuntu-latest` | `android-<version>-<build>`: `com.jfversluis.mauigallery-Signed.aab` | Your upload key |
| iOS (IPA) | `xcode-27` | `ios-<version>-<build>`: `MauiGallery.ipa` | Apple Distribution certificate and App Store provisioning profile |
| Mac Catalyst (PKG) | `xcode-27` | `maccatalyst-<version>-<build>`: `MauiGallery-<version>.pkg` (universal: Apple silicon and Intel) | Apple Distribution certificate, Mac Installer Distribution certificate and Mac App Store provisioning profile |
| Windows (MSIX) | `windows-latest` | `windows-<version>-<build>-win-x64` and `-win-arm64`: one `.msix` each | Unsigned: the Microsoft Store signs it. Needs the Partner Center identity variables |

When a job's secrets are missing it still builds, warns, and uploads a package that the store won't accept: the AAB
is signed with a debug key, the IPA and PKG are unsigned. That keeps the workflow useful on forks and before the
secrets exist. Without `MAPS_API_KEY` (Android) or `AZURE_MAPS_KEY` (Windows) the Map sample shows a message instead
of a map.

The Mac Catalyst job signs with a provisioning profile, which switches the app to
`Platforms/MacCatalyst/Entitlements.AppStore.plist`: the sandbox entitlements plus the keychain access group that
SecureStorage needs. Unsigned and ad-hoc builds (CI, local Release builds) use `Entitlements.plist` without it, and
Debug builds use `Entitlements.Debug.plist`, which adds the network server entitlement for DevFlow.

## Secrets

Add these under Settings > Secrets and variables > Actions. Each job only needs its own secrets.

The Apple team ID defaults to `AY5KBJ6RN9` in the workflow; set the `APPLE_TEAM_ID` repository **variable** to use
another team. The workflow uses it to pick the right certificates from the `.p12` files.

### Android

| Secret | What it is |
| --- | --- |
| `ANDROID_KEYSTORE_BASE64` | The upload keystore, base64 encoded |
| `ANDROID_KEYSTORE_PASSWORD` | The keystore password |
| `ANDROID_KEY_ALIAS` | The key's alias in the keystore |
| `ANDROID_KEY_PASSWORD` | The key's password |
| `MAPS_API_KEY` | Google Maps SDK for Android key for the Map sample (also used by CI) |

Create the upload key once and keep a backup outside GitHub: Google Play App Signing holds the app signing key, and
this key only proves that uploads come from you.

```bash
keytool -genkeypair -v -keystore upload.keystore -alias upload -keyalg RSA -keysize 2048 -validity 10000
base64 -i upload.keystore | pbcopy    # paste as ANDROID_KEYSTORE_BASE64
```

For `MAPS_API_KEY`, create a key in the Google Cloud console with the Maps SDK for Android enabled, and restrict it to
Android apps with the package name `com.jfversluis.mauigallery` and the SHA-1 of the **app signing** certificate
(Play Console > Test and release > App integrity), plus your upload key's SHA-1 for testing. Also restrict it to the
**Maps SDK for Android** API only. See [Map keys and costs](#map-keys-and-costs).

### Apple (iOS and Mac Catalyst)

| Secret | What it is |
| --- | --- |
| `APPLE_CERTIFICATE_P12_BASE64` | The **Apple Distribution** certificate with its private key, exported as `.p12`, base64 encoded. Signs both the iOS and the Mac app |
| `APPLE_CERTIFICATE_PASSWORD` | The `.p12` password |
| `APPLE_INSTALLER_CERTIFICATE_P12_BASE64` | The **Mac Installer Distribution** certificate (shown as "3rd Party Mac Developer Installer") with its private key, as `.p12`, base64 encoded. Signs the `.pkg` |
| `APPLE_INSTALLER_CERTIFICATE_PASSWORD` | Optional: its password, if it differs from `APPLE_CERTIFICATE_PASSWORD` |
| `APPLE_PROVISIONING_PROFILE_IOS_BASE64` | An **App Store Connect** distribution profile for iOS, App ID `com.jfversluis.mauigallery`, base64 encoded |
| `APPLE_PROVISIONING_PROFILE_MAC_BASE64` | A **Mac App Store Connect** distribution profile for Mac Catalyst, the same App ID, base64 encoded |

1. In [Certificates, Identifiers & Profiles](https://developer.apple.com/account/resources), make sure the App ID
   `com.jfversluis.mauigallery` exists with **Mac Catalyst** enabled (Mac Catalyst apps share the iOS App ID).
2. Create the certificates in Xcode (Settings > Accounts > Manage Certificates > + > Apple Distribution, and
   Mac Installer Distribution) or on the website, then export each from Keychain Access (right-click > Export, as
   `.p12` with a password).
3. Create two distribution profiles for the App ID: **App Store Connect** (platform iOS) and **Mac App Store Connect**
   (platform macOS, type Mac Catalyst). Download them.
4. Base64 encode each file and paste it as the secret: `base64 -i file.p12 | pbcopy`.

Certificates expire after a year and profiles when their certificate does: renew them and update the secrets.

## Store checklists

### App Store (iPhone and iPad)

1. Create the app in [App Store Connect](https://appstoreconnect.apple.com) once: platform iOS, name
   ".NET MAUI Gallery", bundle ID `com.jfversluis.mauigallery`, SKU `mauigallery`. Add macOS to the same record for
   the Mac app.
2. Upload `MauiGallery.ipa` with [Transporter](https://apps.apple.com/app/transporter/id1450874784) or
   `xcrun altool --upload-app -t ios -f MauiGallery.ipa --apiKey <key> --apiIssuer <issuer>`.
3. App information: category **Developer Tools** (secondary: Education), content rights (no third-party content
   that needs rights), age rating questionnaire (all "None"; rated 4+).
4. App privacy: **Data not collected** (see [privacy.md](privacy.md) and [store-listing.md](store-listing.md)).
   Privacy policy URL: `https://github.com/jfversluis/maui-gallery/blob/main/docs/privacy.md`.
5. Version page: screenshots (6.9" iPhone and 13" iPad), promotional text, description, keywords, support URL,
   "What's new", then pick the uploaded build.
6. Export compliance: the app sets `ITSAppUsesNonExemptEncryption` to false (it only uses the OS's HTTPS and
   Keychain), so App Store Connect doesn't ask.
7. App Review notes: no account needed; some samples ask for location, camera, microphone and motion permissions
   only when used; the Map sample needs network access.
8. Submit for review. Optionally test the build in TestFlight first.

### Mac App Store

1. In the same App Store Connect record, add the macOS platform.
2. Upload `MauiGallery-<version>.pkg` with Transporter (or `xcrun altool --upload-app -t macos -f <pkg> ...`).
3. Category is set in the app (`LSApplicationCategoryType` = Developer Tools). Fill in the macOS version page:
   screenshots (2880×1800), description, keywords and "What's new" (shared texts can be copied from iOS).
4. App Review notes: same as iOS.
5. Submit for review.

### Google Play

0. **Before building the release, set up the Google Maps key**: create it, restrict it as described under
   [Map keys and costs](#map-keys-and-costs), and store it as the `MAPS_API_KEY` secret. Without it the Map sample on
   Android shows a "needs an API key" message instead of a map.
1. Create the app in the [Play Console](https://play.google.com/console) once: name ".NET MAUI Gallery", app, free.
   Opt in to Play App Signing (the default), and register your upload key when asked.
2. Complete App content: privacy policy URL (above), ads (no ads), app access (no login needed), content rating
   questionnaire (category Reference/Utility, all answers "No"), target audience (13+, not designed for children),
   data safety (see [store-listing.md](store-listing.md): no data collected or shared), government app (no).
3. Store listing: name, short and full description, icon (512×512, export from the app icon), feature graphic
   (1024×500), phone screenshots, category **Tools**, contact email and website.
4. Create a release (Internal testing first, then Production) and upload `com.jfversluis.mauigallery-Signed.aab`.
   The bundle targets the latest Android API level (the .NET 11 default, currently 37), which satisfies Google Play's
   target API requirement. Minimum is Android 7.0 (API 24).
5. Permission declarations: none of the permissions need a declaration form (no background location, no SMS/call
   log, no all-files access).
6. Release notes from "What's new", review and roll out.

### Microsoft Store

1. In [Partner Center](https://partner.microsoft.com/dashboard), reserve the app name and open Product management >
   Product identity.
2. Add three repository variables (Settings > Secrets and variables > Actions > Variables) with the values shown
   there: `WINDOWS_IDENTITY_NAME` (Package/Identity/Name), `WINDOWS_PUBLISHER` (Package/Identity/Publisher, starting
   with `CN=`) and `WINDOWS_PUBLISHER_DISPLAY_NAME` (Package/Properties/PublisherDisplayName). They aren't secrets.
   Without them the job still builds, with a placeholder identity the Store rejects.
3. Run the workflow, download both `windows-*` artifacts and upload the two `.msix` files (x64 and Arm64) to the
   submission's Packages page. The Store signs them; no certificate is needed.
4. The package version is `Version` from `Directory.Build.props` plus `.0` (the Store requires the last part to be 0),
   so bump `Version` for every Store submission.
5. Store listing texts are in [store-listing.md](store-listing.md); the app collects no data
   ([privacy.md](privacy.md)). Declare the location, microphone and webcam capabilities the samples use.
6. The Map sample ships without an Azure Maps key: on Windows it asks people to paste their own, which it keeps in
   memory only. To ship one anyway, add an `AZURE_MAPS_KEY` secret (see [Map keys and costs](#map-keys-and-costs)).

## Map keys and costs

Any key that ships inside an app can be read out of the package, so treat both map keys as public and limit what they
can do rather than relying on keeping them secret.

- **Google Maps (Android, `MAPS_API_KEY`).** The Maps SDK for Android has unlimited free map loads when no map ID is
  used, which is how .NET MAUI's `Map` loads it. Google still requires a billing account on the project. Restrict the
  key to the Android app (package name and certificate SHA-1s) **and** to the Maps SDK for Android API, so a copied key
  can't call paid APIs. Add a budget alert in Cloud Billing as a safety net; budgets alert but don't stop spending.
- **Azure Maps (Windows, `AZURE_MAPS_KEY`, optional).** Store builds leave it out: the Map page lets people paste their
  own key. If you do ship one, create an Azure Maps account (Gen2 pricing) and copy its primary key
  (Authentication). Base map tiles have a free monthly allowance (5,000 transactions of up to 15 tiles each), then
  they're billed per 1,000 transactions; traffic has a smaller allowance. To rule out charges, put the account in a
  subscription with a spending limit, such as a Visual Studio subscription's monthly Azure credit: Azure disables its
  resources when the credit runs out instead of billing you. Otherwise, add a budget alert in Cost Management.

