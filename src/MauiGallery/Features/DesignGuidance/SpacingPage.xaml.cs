using System.ComponentModel;
using MauiGallery.Core.Layout;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.DesignGuidance;

public sealed partial class SpacingPage : SamplePage
{
    public SpacingPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        PageBody.PropertyChanged += OnPageBodyPropertyChanged;
        Loaded += (_, _) => UpdateReadout();
        OnTokensChanged(this, EventArgs.Empty);
    }

    private void OnPageBodyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Padding))
        {
            UpdateReadout();
        }
    }

    private void UpdateReadout()
    {
        if (Window is not { Width: > 0 } window)
        {
            return;
        }

        var sizeClass = Breakpoints.Classify(window.Width);
        var padding = PageBody.Padding;
        PaddingReadout.Text = $"This window is {window.Width:0} wide, so it's {sizeClass} and this page's padding is {padding.Left:0}, {padding.Top:0}, {padding.Right:0}, {padding.Bottom:0}.";

        foreach (var (row, rowClass) in new[] { (CompactRow, SizeClass.Compact), (MediumRow, SizeClass.Medium), (ExpandedRow, SizeClass.Expanded) })
        {
            if (rowClass == sizeClass)
            {
                row.SetDynamicResource(BackgroundColorProperty, "ColorSelection");
            }
            else
            {
                row.BackgroundColor = Colors.Transparent;
            }
        }
    }


    private void OnTokensChanged(object? sender, EventArgs e)
    {
        // Options raise their events while the page is still loading.
        if (TokensStack is null || TokensSpacing?.SelectedItem is not string spacing || TokensPadding?.SelectedItem is not string padding)
        {
            return;
        }

        TokensStack.Spacing = Token(spacing);
        TokensSample.Padding = Token(padding);
        TokensExample.Output = $"Spacing {Token(spacing):0} · Padding {Token(padding):0}";
    }

    private static double Token(string key) =>
        Application.Current?.Resources.TryGetValue(key, out var value) == true && value is double size ? size : 0;
}
