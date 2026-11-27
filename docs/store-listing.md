# Store listing

Texts and answers for the App Store, Mac App Store and Google Play. The Microsoft Store listing can reuse the Google
Play texts. See [releasing.md](releasing.md) for how to build and upload, and [privacy.md](privacy.md) for the
privacy policy.

Apple doesn't allow other platforms' names (such as Android) in App Store metadata, so the App Store texts avoid
them; the Google Play texts don't mention Apple platforms either.

## Basics

| Field | Value |
| --- | --- |
| App name | .NET MAUI Gallery |
| Home screen name | MAUI Gallery (iPhone, iPad and Android; the full name doesn't fit under the icon) |
| Bundle ID / package name | `com.jfversluis.mauigallery` |
| App Store Connect | App ID 6839980440 (iOS and macOS in one record, SKU `mauigallery`) |
| Price | Free, no in-app purchases, no ads |
| Category | App Store and Mac App Store: **Developer Tools** (secondary: Education). Google Play: **Tools**. Microsoft Store: Developer tools |
| Support URL | https://github.com/jfversluis/maui-gallery/issues |
| Marketing / website URL | https://github.com/jfversluis/maui-gallery |
| Privacy policy URL | https://github.com/jfversluis/maui-gallery/blob/main/docs/privacy.md |
| Copyright | Gerald Versluis |
| Languages | English |

## App Store and Mac App Store

The same texts are in [`eng/release/app-store/metadata/en-US`](../eng/release/app-store/metadata/en-US), one file per
field, for the App Store listing workflow ([releasing.md](releasing.md#apple-screenshots-and-texts-optional)). Change
both together.

**Subtitle** (30 characters max)

> Every control, live and native

**Promotional text** (170 max, can change without a new build)

> Over 100 interactive samples of .NET MAUI controls, layouts, graphics and device APIs. Change the options, see the
> native view, and copy the XAML or C#.

**Keywords** (100 max, comma separated; the words in the name are already indexed)

> dotnet,xaml,csharp,c#,controls,ui,samples,developer,cross-platform,blazor,mvvm,learn,code,components

**Description**

> The .NET MAUI Gallery is the companion app for developers building with .NET MAUI, Microsoft's open-source
> framework for native apps in C# and XAML. Explore the controls, layouts and APIs in one place, running live on your
> device.
>
> SEE IT RUN, NATIVELY
> Every sample is a real native control, not a picture of one. A badge shows the native view each control becomes on
> this device, so you can see exactly what .NET MAUI renders.
>
> PLAY WITH IT
> Change the options next to each sample and watch the control update. Switch between the platform's default look
> and a styled one, mirror the layout for right-to-left languages, and try light and dark themes.
>
> COPY THE CODE
> The XAML and C# for each sample update as you change its options, so the code always matches what you see. Copy it
> straight into your app.
>
> WHAT'S INSIDE
> • Controls: buttons, text input, pickers, collections, carousels, swipe and refresh, dialogs, menus and toolbars
> • Layouts: Grid, StackLayout, FlexLayout, AbsoluteLayout, Border, ScrollView and custom layouts
> • Graphics: shapes, paths, brushes, shadows, clipping, transforms and the GraphicsView canvas
> • Fundamentals: data binding, converters, MVVM commands, styles, themes, visual states, triggers, behaviors,
>   animations, gestures, drag and drop, and accessibility
> • Navigation: Shell, tabs, flyouts, navigation pages, multiple windows and deep links
> • Platform APIs: geolocation, sensors, haptics, the camera and media picker, battery, connectivity, clipboard,
>   sharing, text-to-speech, secure storage and more
> • Web and native power: WebView, HybridWebView, BlazorWebView, handlers, platform-specifics and native views
> • Design guidance: color, typography, icons, spacing and responsive layouts
>
> Search everything, save favorites, and pick up where you left off. On iPad and Mac the gallery uses a sidebar and
> multiple windows; on iPhone it fits in one hand.
>
> PRIVATE BY DESIGN
> No accounts, no analytics, no ads. Samples that use the camera, location or sensors only run when you open them,
> and nothing leaves your device.
>
> The gallery is open source: browse the code and contribute at github.com/jfversluis/maui-gallery.
>
> .NET MAUI and .NET are trademarks of Microsoft.

**What's new in 1.0**

> The first release of the .NET MAUI Gallery: over 100 interactive samples of .NET MAUI controls, layouts, graphics,
> navigation and device APIs, each with live options and copyable XAML and C#.

## Google Play

**Short description** (80 max)

> Explore .NET MAUI controls and APIs, running natively, with the code to copy.

**Full description** (4000 max)

Use the App Store description above, with these changes:

- In the last paragraph of "What's inside", replace "On iPad and Mac the gallery uses a sidebar and multiple windows;
  on iPhone it fits in one hand." with "On tablets and foldables the gallery uses a sidebar; on phones it fits in one
  hand."

**Release notes for 1.0**: the "What's new in 1.0" text above. The Release workflow uploads them from
[`eng/release/whatsnew/whatsnew-en-US`](../eng/release/whatsnew/whatsnew-en-US) (500 characters max), so update that
file with each version.

**Graphics**: app icon 512×512 (render `src/MauiGallery/Resources/AppIcon` at 512 px), feature graphic 1024×500
(the brand gradient with the icon mark and the app name), phone screenshots from the store screenshot set.

## Age ratings

The app has no user-generated content, no chat, no purchases, no gambling, no violence and no mature themes.

- **App Store**: answer "None" or "No" to every content question. One question needs a decision: the Launcher and
  browser sample opens any address you type in the in-app browser (Safari View Controller), and the WebView sample
  shows a few fixed public pages whose links can be followed. If Apple asks whether the app offers
  **unrestricted web access**, answering Yes raises the rating to the highest age band. The recommended answer is
  No (the app isn't a browser; the sample demonstrates the API), and to mention the browser sample in the review
  notes. Expected rating: 4+.
- **Google Play (IARC)**: category Reference, News, or Educational (or Utility, Productivity, Communication, or
  Other); answer No to violence, sexuality, language, controlled substances, gambling, user interaction and sharing
  of location with other users. Expected rating: Everyone / PEGI 3.

## Privacy answers

### App Store privacy ("nutrition label")

- Do you or your third-party partners collect data from this app? **No, we do not collect data from this app.**
- Tracking: **No.** The app's privacy manifest (`PrivacyInfo.xcprivacy`) declares no tracking, no tracking domains and
  no collected data types. It lists the required-reason APIs .NET and .NET MAUI use: file timestamps (C617.1, 3B52.1),
  system boot time (35F9.1), disk space (E174.1) and user defaults (CA92.1).

Apple's definition of "collect" is transmitting data off the device in a way that lets you or partners access it
beyond what's needed to serve a request in real time. The gallery sends nothing to its developer. Map tiles and
address lookups go to Apple's own services, like in any app that shows a map.

### Google Play data safety

- Does your app collect or share any of the required user data types? **No.**
- Is all of the user data collected by your app encrypted in transit? Not applicable (no data collected). The
  network requests the samples make (web pages, images, map tiles) use HTTPS.
- Do you provide a way for users to request that their data is deleted? Not applicable: nothing is stored off the
  device; uninstalling removes everything.

Before submitting, check the Map sample: on Android it uses Google's Maps SDK, which
[collects request metadata, an SDK identifier, the IP address and crash data](https://developers.google.com/maps/documentation/android-sdk/play-data-disclosure)
on its own. Decide whether the form and the privacy policy must declare that.

Google's guidance: data processed only on the device, and data sent to a service only to fulfil a request in real
time without being stored, isn't "collected". Location in the Geolocation, Geocoding and Map samples is shown on
screen only; geocoding goes to the platform's geocoder.

### Permissions (for review notes and Play's permission questions)

Every permission is requested only when you use the sample that needs it, and its data stays on the device.

| Permission | Platform | Used by | Why |
| --- | --- | --- | --- |
| Location (when in use) | All | Geolocation, Geocoding, Map | Show your position and address on screen |
| Camera | All | Media picker; Flashlight (Android) | Take a photo or video and show it; turn on the torch |
| Microphone | All | Media picker, Permissions | Record sound with a video; show how to request a permission |
| Motion (iOS) | iOS | Sensors | Read the barometer |
| Internet, network state | Android | WebView, Image, Map, Connectivity | Load web pages, images and map tiles; show the connection state |
| Vibrate | Android | Haptics and vibration | Vibrate and play haptic feedback |
| Flashlight | Android | Flashlight | Turn the torch on and off |
| Battery stats | Android | Battery | Read the charge level and state (.NET MAUI's Battery API declares it) |
| High sampling rate sensors | Android | Sensors | Read sensors at the "Fastest" speed |

None of the hardware is required (`uses-feature required="false"`): samples explain what's missing instead.
There is no background location, no contacts, no SMS or call log, and no broad file access (the file picker uses the
system picker).

## Screenshots

Captured from the running app with DevFlow, without device frames. The App Store and Mac sets are in
[`eng/release/app-store/screenshots`](../eng/release/app-store/screenshots), where the listing workflow uploads them
from:

| Store | Device class | Size |
| --- | --- | --- |
| App Store | iPhone 6.9" | 1320×2868 |
| App Store | iPad 13" | 2064×2752 |
| Mac App Store | Mac | 2880×1800 |
| Google Play | Phone (Pixel 10) | 1080×2424 |
| Microsoft Store | Desktop | 1366×768 or larger, 1920×1080 recommended (not captured yet) |

The pages name each control's native type on every platform (for example "Android · RecyclerView"), and App Review
guideline 2.3.10 discourages other platforms' names in App Store screenshots. The App Store shots therefore start at
each page's first sample, below those chips; if review still objects, pick shots without platform names.

Suggested order: Home, a control page with options and code (Button), the native view badge, CollectionView,
Graphics, Platform APIs (Sensors), Blazor or HybridWebView, and dark mode or Settings. Caption ideas: "Every control,
live and native", "Change options, watch the code update", "See the native view behind every control", "Collections,
carousels and swipe", "Draw with shapes, brushes and GraphicsView", "Sensors, location and more device APIs",
"Mix in web UI with Blazor", "Light and dark, phone to desktop".
