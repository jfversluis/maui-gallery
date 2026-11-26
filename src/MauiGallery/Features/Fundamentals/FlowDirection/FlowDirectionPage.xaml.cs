using System.Globalization;
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

    public FlowDirectionPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        MirrorExample.NativeTarget = MirrorSample;
        AlignExample.NativeTarget = AlignSample;
        TextExample.NativeTarget = MessageEntry;
        OnLanguageChanged(this, EventArgs.Empty);
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
