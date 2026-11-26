using System.Globalization;
using MauiGallery.Features.Samples;
using Microsoft.Maui.Layouts;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class AbsoluteLayoutPage : SamplePage
{
    public AbsoluteLayoutPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        AbsoluteExample.NativeTarget = AbsoluteSample;
        ProportionalExample.NativeTarget = ProportionalSample;
        BadgeExample.NativeTarget = BadgeSample;
        OnAbsoluteChanged(this, EventArgs.Empty);
        OnProportionalChanged(this, EventArgs.Empty);
        OnBadgeCountChanged(this, EventArgs.Empty);
    }

    private void OnAbsoluteChanged(object? sender, EventArgs e)
    {
        var bounds = new Rect(Math.Round(AbsoluteX.Value), Math.Round(AbsoluteY.Value), 80, 50);
        AbsoluteLayout.SetLayoutBounds(AbsoluteTile, bounds);
        AbsoluteBounds.Value = Format(bounds, ",");
        AbsoluteBoundsCode.Value = Format(bounds, ", ");
    }

    private void OnProportionalChanged(object? sender, EventArgs e)
    {
        var x = Math.Round(ProportionalX.Value, 2);
        var y = Math.Round(ProportionalY.Value, 2);

        // With All, width and height are fractions of the layout too; otherwise they stay in device-independent units.
        var (flags, bounds) = ProportionalSize.IsToggled
            ? (AbsoluteLayoutFlags.All, new Rect(x, y, 0.4, 0.3))
            : (AbsoluteLayoutFlags.PositionProportional, new Rect(x, y, 80, 50));

        AbsoluteLayout.SetLayoutFlags(ProportionalTile, flags);
        AbsoluteLayout.SetLayoutBounds(ProportionalTile, bounds);
        ProportionalTileText.Text = ProportionalSize.IsToggled ? "40% × 30%" : "80 × 50";
        ProportionalBounds.Value = Format(bounds, ",");
        ProportionalBoundsCode.Value = Format(bounds, ", ");
        ProportionalFlags.Value = flags;
    }

    private void OnBadgeCountChanged(object? sender, EventArgs e)
    {
        var count = (int)BadgeCount.Value;
        CountBadge.IsVisible = count > 0;
        CountText.Text = count > 99 ? "99+" : count.ToString(CultureInfo.CurrentCulture);
        BadgeCountText.Value = CountText.Text;
        BadgeVisible.Value = CountBadge.IsVisible;
        SemanticProperties.SetDescription(BadgeSample, count > 0 ? $"Ada Lovelace, {count} unread" : "Ada Lovelace");
    }

    private static string Format(Rect bounds, string separator) =>
        string.Join(separator, new[] { bounds.X, bounds.Y, bounds.Width, bounds.Height }.Select(v => v.ToString("0.##", CultureInfo.InvariantCulture)));
}
