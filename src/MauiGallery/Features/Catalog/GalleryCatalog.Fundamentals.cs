using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Fundamentals;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] FundamentalsItems() =>
    [
        new()
        {
            Id = "animations",
            Title = "Animations",
            Description = "Fades, scales, rotates and moves views with easing, together or in sequence, or with an Animation of your own.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.SlideTransition24,
            PageType = typeof(AnimationsPage),
            Keywords = ["animation", "FadeToAsync", "ScaleToAsync", "RotateToAsync", "RotateYToAsync", "TranslateToAsync", "LayoutToAsync", "Easing", "Animation", "Commit", "AbortAnimation", "CancelAnimations", "CancellationToken", "Task.WhenAll", "motion", "reduce motion", "tween"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CADisplayLink ticker"),
                new(GalleryPlatforms.Android, "ValueAnimator ticker"),
                new(GalleryPlatforms.MacCatalyst, "CADisplayLink ticker"),
                new(GalleryPlatforms.Windows, "CompositionTarget.Rendering ticker"),
            ],
            RelatedIds = ["transforms", "graphics-view", "gestures"],
            DocsUrl = new(DocsRoot + "user-interface/animation/basic"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.viewextensions"),
            SourceUrl = new(SourceRoot + "Fundamentals/Animations"),
        },
        new()
        {
            Id = "gestures",
            Title = "Gestures",
            Description = "Responds to taps, long presses, drags, pinches, swipes and the mouse pointer on any view.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.Gesture24,
            PageType = typeof(GesturesPage),
            Keywords = ["gesture", "GestureRecognizers", "TapGestureRecognizer", "NumberOfTapsRequired", "double tap", "right-click", "Buttons", "LongPressGestureRecognizer", "long press", "press and hold", "PanGestureRecognizer", "drag", "PinchGestureRecognizer", "pinch", "zoom", "SwipeGestureRecognizer", "swipe", "PointerGestureRecognizer", "hover", "mouse", "pen", "touch"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIGestureRecognizer subclasses"),
                new(GalleryPlatforms.Android, "GestureDetector and ScaleGestureDetector"),
                new(GalleryPlatforms.MacCatalyst, "UIGestureRecognizer subclasses"),
                new(GalleryPlatforms.Windows, "UIElement gesture and pointer events"),
            ],
            RelatedIds = ["drag-and-drop", "swipe-view", "graphics-view", "animations"],
            DocsUrl = new(DocsRoot + "fundamentals/gestures/tap"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.gesturerecognizer"),
            SourceUrl = new(SourceRoot + "Fundamentals/Gestures"),
            IsUpdated = true,
        },
        new()
        {
            Id = "drag-and-drop",
            Title = "Drag and drop",
            Description = "Drags text and your own data between views, refuses drops that don't fit and reorders items.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.Drag24,
            PageType = typeof(DragAndDropPage),
            Keywords = ["drag", "drop", "DragGestureRecognizer", "DropGestureRecognizer", "DataPackage", "Properties", "DragOver", "DragLeave", "DropCompleted", "AcceptedOperation", "AllowDrop", "CanDrag", "reorder", "move", "sort"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIDragInteraction / UIDropInteraction"),
                new(GalleryPlatforms.Android, "View.StartDragAndDrop / OnDragListener"),
                new(GalleryPlatforms.MacCatalyst, "UIDragInteraction / UIDropInteraction"),
                new(GalleryPlatforms.Windows, "UIElement.CanDrag / AllowDrop"),
            ],
            RelatedIds = ["gestures", "collection-view", "bindable-layout"],
            DocsUrl = new(DocsRoot + "fundamentals/gestures/drag-and-drop"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.draggesturerecognizer"),
            SourceUrl = new(SourceRoot + "Fundamentals/DragAndDrop"),
        },
        new()
        {
            Id = "accessibility",
            Title = "Accessibility",
            Description = "Names, headings, announcements and hidden decoration that make an app work with a screen reader.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.Accessibility24,
            PageType = typeof(AccessibilityPage),
            Keywords = ["accessibility", "a11y", "screen reader", "VoiceOver", "TalkBack", "Narrator", "SemanticProperties", "Description", "Hint", "HeadingLevel", "heading", "SemanticScreenReader", "Announce", "SetSemanticFocus", "focus", "AutomationProperties", "IsInAccessibleTree", "ExcludedWithChildren", "decorative"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIAccessibility (label, hint, traits)"),
                new(GalleryPlatforms.Android, "AccessibilityNodeInfo (description, hint, heading)"),
                new(GalleryPlatforms.MacCatalyst, "UIAccessibility (label, hint, traits)"),
                new(GalleryPlatforms.Windows, "AutomationProperties (Name, HelpText, HeadingLevel)"),
            ],
            RelatedIds = ["fonts", "label", "image-button", "typography"],
            DocsUrl = new(DocsRoot + "fundamentals/accessibility"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.semanticproperties"),
            SourceUrl = new(SourceRoot + "Fundamentals/Accessibility"),
        },
        new()
        {
            Id = "fonts",
            Title = "Fonts",
            Description = "Embeds custom fonts, sets size and weight, follows the user's text size and draws icons from a font.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.TextFontSize24,
            PageType = typeof(FontsPage),
            Keywords = ["font", "FontFamily", "ConfigureFonts", "AddFont", "embedded font", "custom font", "ttf", "otf", "FontSize", "FontAttributes", "bold", "italic", "NamedSize", "FontAutoScalingEnabled", "Dynamic Type", "text size", "FontImageSource", "icon font", "glyph", "Fluent icons"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIFont (scaled with UIFontMetrics)"),
                new(GalleryPlatforms.Android, "Typeface, TextView.SetTextSize (sp)"),
                new(GalleryPlatforms.MacCatalyst, "UIFont (scaled with UIFontMetrics)"),
                new(GalleryPlatforms.Windows, "FontFamily, IsTextScaleFactorEnabled"),
            ],
            RelatedIds = ["typography", "label", "image-button", "accessibility"],
            DocsUrl = new(DocsRoot + "user-interface/fonts"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.fontimagesource"),
            SourceUrl = new(SourceRoot + "Fundamentals/Fonts"),
        },
        new()
        {
            Id = "flow-direction",
            Title = "Flow direction",
            Description = "Mirrors layouts and aligns text for right-to-left languages such as Arabic and Hebrew.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.TextParagraphDirection24,
            PageType = typeof(FlowDirectionPage),
            Keywords = ["FlowDirection", "RightToLeft", "LeftToRight", "MatchParent", "RTL", "LTR", "right-to-left", "bidi", "Arabic", "Hebrew", "Persian", "Urdu", "mirror", "localization", "Start", "End", "TextInfo.IsRightToLeft"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIView.SemanticContentAttribute"),
                new(GalleryPlatforms.Android, "View.LayoutDirection"),
                new(GalleryPlatforms.MacCatalyst, "UIView.SemanticContentAttribute"),
                new(GalleryPlatforms.Windows, "FrameworkElement.FlowDirection"),
            ],
            RelatedIds = ["grid", "stack-layout", "flex-layout", "label", "entry"],
            DocsUrl = new(DocsRoot + "fundamentals/localization#right-to-left-localization"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.visualelement.flowdirection"),
            SourceUrl = new(SourceRoot + "Fundamentals/FlowDirection"),
        },
    ];
}
