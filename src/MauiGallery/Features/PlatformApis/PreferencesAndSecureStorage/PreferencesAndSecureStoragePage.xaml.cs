using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class PreferencesAndSecureStoragePage : SamplePage
{
    private readonly SecretsViewModel _secrets;

    public PreferencesAndSecureStoragePage(SamplePageServices services, SettingsStoreViewModel settings, SecretsViewModel secrets)
        : base(services)
    {
        InitializeComponent();

        PreferencesSample.BindingContext = settings;
        PreferencesExample.NativeTarget = PreferencesRemove;
        settings.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SettingsStoreViewModel.Stored))
            {
                PreferencesExample.Output = $"Preferences \"{SettingsStoreViewModel.SharedName}\": {settings.Stored.Count(s => s.Contains(" = ", StringComparison.Ordinal))} keys";
            }
        };

        _secrets = secrets;
        SecureSample.BindingContext = secrets;
        SecureExample.NativeTarget = SecureSave;
        secrets.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SecretsViewModel.Result))
            {
                SecureExample.Output = secrets.Result;
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _secrets.ReadCommand.Execute(null);
    }
}
