# Privacy policy

_Last updated: November 2026_

The .NET MAUI Gallery ("the app") is a sample app that demonstrates .NET MAUI controls and APIs.

## Data we collect

**None.** The app does not collect, store on a server, sell or share any personal information. It has no accounts,
no analytics and no advertising. The only exception is Google's Maps SDK on Android, described under
[Network access](#network-access).

## Data stored on your device

The app stores your theme choice, favorites and recently visited pages on your device using the platform's
standard preferences storage. The Preferences and Secure storage samples store the values you type into them, in
the same storage and in the platform's keychain or keystore. This data never leaves your device and is removed when
you uninstall the app.

## Device features

Some samples demonstrate device features such as location, the camera, sensors or the clipboard. These samples only
run when you open them and interact with them, they ask for permission where the platform requires it, and any data
they use stays on your device and is not stored by the app.

## Network access

The app works offline. A few samples load content from the internet when you open them:

- The WebView sample shows public web pages, such as dotnet.microsoft.com and github.com.
- The Image sample loads public pictures from raw.githubusercontent.com.
- The Map sample shows map tiles from Apple Maps (iOS and Mac), Google Maps (Android) or Azure Maps (Windows), and
  the Geocoding sample uses the platform's address lookup. Those services receive the map area or the address you
  look up, as with any maps app, under Apple's, Google's and Microsoft's privacy policies.

On Android, the map is drawn by Google's Maps SDK. When you open the Map sample, that SDK itself sends Google request
metadata (such as the device model and OS version), a pseudonymous Maps SDK identifier, your IP address, crash data
and how you pan and zoom the map, which Google uses to run and improve its services. See
[Google's disclosure](https://developers.google.com/maps/documentation/android-sdk/play-data-disclosure) and
[Google's privacy policy](https://policies.google.com/privacy). The gallery doesn't receive any of this data.

These requests are ordinary web requests: the app doesn't add any information about you to them.

## Links

Links to documentation and source code open in your browser. Those websites have their own privacy policies.

## Contact

Questions about this policy: open an issue at https://github.com/jfversluis/maui-gallery/issues.
