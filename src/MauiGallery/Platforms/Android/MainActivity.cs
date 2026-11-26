using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace MauiGallery;

// UiMode is deliberately not handled here: Android recreates the activity when light/dark changes,
// so native controls on the stage are re-inflated with the night Material palette.
[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [Intent.CategoryDefault, Intent.CategoryBrowsable],
    DataScheme = "mauigallery")]
public sealed class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        // MAUI only reads the system theme again in OnConfigurationChanged, which a recreated activity never gets.
        // Without this, "Use system setting" keeps the old theme while the native controls switch palette.
        IPlatformApplication.Current?.Application?.ThemeChanged();
        base.OnCreate(savedInstanceState);
    }
}
