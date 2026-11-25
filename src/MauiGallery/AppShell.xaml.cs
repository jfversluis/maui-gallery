using MauiGallery.Features.Spike;

namespace MauiGallery;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("detail", typeof(SpikePage));
    }

    public void ApplyMode(double width)
    {
        var phone = DeviceInfo.Idiom == DeviceIdiom.Phone;
        var mode = phone ? FlyoutBehavior.Disabled : width >= 1100 ? FlyoutBehavior.Locked : FlyoutBehavior.Flyout;
        if (FlyoutBehavior != mode)
        {
            FlyoutBehavior = mode;
            FlyoutIsPresented = false;
        }

        foreach (var tab in Items.SelectMany(i => i.Items))
        {
            SetTabBarIsVisible(tab, phone);
        }
        ModeLabel.Text = $"{width:0} → {mode}";
    }

    private async void OnNav(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string route })
        {
            await GoToAsync(route);
            if (FlyoutBehavior == FlyoutBehavior.Flyout)
            {
                FlyoutIsPresented = false;
            }
        }
    }
}
