using System.Globalization;
using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class FlowDirectionPage : SamplePage
{
    private static readonly (string Culture, string Greeting, string Placeholder)[] Languages =
    [
        ("en-US", "Welcome to the MAUI Gallery. You have 3 new messages.", "Type a message"),
        ("ar-EG", "مرحبًا بك في معرض MAUI. لديك 3 رسائل جديدة.", "اكتب رسالة"),
        ("he-IL", "ברוכים הבאים לגלריית MAUI. יש לך 3 הודעות חדשות.", "כתוב הודעה"),
    ];

    private const string StuckMirroredNotice =
        "On iOS and Mac Catalyst, .NET MAUI 11 doesn't update child views when a view goes from RightToLeft back to "
        + "MatchParent, so this sample stays mirrored. Set LeftToRight to switch it back.";

    private readonly Dictionary<VisualElement, (ControlExample Example, FlowDirection Previous)> _directionSamples = [];

    public FlowDirectionPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        MirrorExample.NativeTarget = MirrorSample;
        AlignExample.NativeTarget = AlignSample;
        TextExample.NativeTarget = MessageEntry;
        OnLanguageChanged(this, EventArgs.Empty);

        if (OperatingSystem.IsIOS() || OperatingSystem.IsMacCatalyst())
        {
            WatchDirection(MirrorSample, MirrorExample);
            WatchDirection(AlignSample, AlignExample);
            WatchDirection(TextSample, TextExample);
        }
    }

    /// <summary>
    /// Explains the sample when it hits the .NET MAUI iOS issue where RightToLeft → MatchParent leaves the children
    /// right to left. That only happens while the parent has no explicit direction: once the stage's LTR/RTL toggle has
    /// been used, the sample follows it, and the change of inherited direction clears the note again.
    /// </summary>
    private void WatchDirection(VisualElement sample, ControlExample example)
    {
        _directionSamples[sample] = (example, sample.FlowDirection);
        sample.PropertyChanged += OnSamplePropertyChanged;
    }

    private void OnSamplePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(FlowDirection) || sender is not VisualElement sample
            || !_directionSamples.TryGetValue(sample, out var watched))
        {
            return;
        }

        var stuck = watched.Previous == FlowDirection.RightToLeft
            && sample.FlowDirection == FlowDirection.MatchParent
            && sample.Parent is VisualElement { FlowDirection: FlowDirection.MatchParent };

        watched.Example.Notice = stuck ? StuckMirroredNotice : null;
        _directionSamples[sample] = (watched.Example, sample.FlowDirection);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        var (name, greeting, placeholder) = Languages[Math.Max(0, LanguagePicker.SelectedIndex)];
        var culture = CultureInfo.GetCultureInfo(name);

        GreetingLabel.Text = greeting;
        MessageEntry.Placeholder = placeholder;
        TextSample.FlowDirection = FollowCulture.IsToggled
            ? culture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight
            : FlowDirection.MatchParent;

        CultureName.Value = name;
        TextExample.Output = FollowCulture.IsToggled
            ? $"{culture.EnglishName}: TextInfo.IsRightToLeft is {culture.TextInfo.IsRightToLeft}, so FlowDirection is {TextSample.FlowDirection}"
            : "FlowDirection is MatchParent: the text keeps the stage's direction";
    }
}
