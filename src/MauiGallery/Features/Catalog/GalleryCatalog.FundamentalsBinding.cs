using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Fundamentals;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] FundamentalsBindingItems() =>
    [
        new()
        {
            Id = "data-binding",
            Title = "Data binding",
            Description = "Keeps UI and data in sync: binding modes, formatting, fallbacks and compiled bindings.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.LinkMultiple24,
            PageType = typeof(DataBindingPage),
            Keywords = ["binding", "bind", "x:Reference", "Mode", "TwoWay", "OneWay", "OneWayToSource", "OneTime", "StringFormat", "FallbackValue", "TargetNullValue", "x:DataType", "compiled bindings", "MauiStrictXamlCompilation", "SetBinding"],
            RelatedIds = ["value-converters", "relative-bindings", "mvvm-commands", "entry", "slider"],
            DocsUrl = new(DocsRoot + "fundamentals/data-binding/"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.binding"),
            SourceUrl = new(SourceRoot + "Fundamentals/DataBinding"),
        },
        new()
        {
            Id = "value-converters",
            Title = "Value converters",
            Description = "Transform values as they flow through a binding, with IValueConverter and MultiBinding.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.ArrowSwap24,
            PageType = typeof(ValueConvertersPage),
            Keywords = ["converter", "IValueConverter", "IMultiValueConverter", "MultiBinding", "ConverterParameter", "Convert", "ConvertBack", "transform", "binding"],
            RelatedIds = ["data-binding", "triggers", "stepper", "checkbox"],
            DocsUrl = new(DocsRoot + "fundamentals/data-binding/converters"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.ivalueconverter"),
            SourceUrl = new(SourceRoot + "Fundamentals/ValueConverters"),
        },
        new()
        {
            Id = "relative-bindings",
            Title = "Relative bindings",
            Description = "Bind to the element itself, an ancestor's view model, or a template's parent with RelativeSource.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.Organization24,
            PageType = typeof(RelativeBindingsPage),
            Keywords = ["RelativeSource", "Self", "AncestorType", "AncestorLevel", "FindAncestor", "TemplatedParent", "TemplateBinding", "ControlTemplate", "item template", "binding"],
            RelatedIds = ["data-binding", "mvvm-commands", "bindable-layout", "collection-view", "radio-button"],
            DocsUrl = new(DocsRoot + "fundamentals/data-binding/relative-bindings"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.relativebindingsource"),
            SourceUrl = new(SourceRoot + "Fundamentals/RelativeBindings"),
        },
        new()
        {
            Id = "mvvm-commands",
            Title = "MVVM and commands",
            Description = "Move state and logic into view models with CommunityToolkit.Mvvm observable properties and commands.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.CodeBlock24,
            PageType = typeof(MvvmCommandsPage),
            Keywords = ["MVVM", "ViewModel", "ICommand", "Command", "RelayCommand", "AsyncRelayCommand", "ObservableProperty", "ObservableObject", "CanExecute", "CommandParameter", "IsRunning", "cancellation", "CommunityToolkit.Mvvm", "INotifyPropertyChanged"],
            RelatedIds = ["data-binding", "relative-bindings", "behaviors", "button", "activity-indicator"],
            DocsUrl = new(DocsRoot + "fundamentals/data-binding/commanding"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.command"),
            SourceUrl = new(SourceRoot + "Fundamentals/MvvmCommands"),
        },
        new()
        {
            Id = "styles-and-resources",
            Title = "Styles and resources",
            Description = "Share colors, values and styles through resource dictionaries, implicit and explicit styles, and style classes.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.StyleGuide24,
            PageType = typeof(StylesAndResourcesPage),
            Keywords = ["Style", "ResourceDictionary", "StaticResource", "DynamicResource", "implicit style", "explicit style", "BasedOn", "StyleClass", "Class", "MergedDictionaries", "Setter", "resources"],
            RelatedIds = ["app-theme", "visual-state-manager", "triggers", "color", "typography"],
            DocsUrl = new(DocsRoot + "user-interface/styles/xaml"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.style"),
            SourceUrl = new(SourceRoot + "Fundamentals/StylesAndResources"),
        },
        new()
        {
            Id = "app-theme",
            Title = "Light and dark themes",
            Description = "Follow and react to the light or dark theme with AppThemeBinding, UserAppTheme and swappable dictionaries.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.DarkTheme24,
            PageType = typeof(AppThemePage),
            Keywords = ["theme", "dark mode", "light mode", "AppThemeBinding", "UserAppTheme", "RequestedTheme", "PlatformAppTheme", "RequestedThemeChanged", "SetAppThemeColor", "DynamicResource", "appearance"],
            RelatedIds = ["styles-and-resources", "color", "visual-state-manager"],
            DocsUrl = new(DocsRoot + "user-interface/system-theme-changes"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.application.userapptheme"),
            SourceUrl = new(SourceRoot + "Fundamentals/AppTheme"),
        },
        new()
        {
            Id = "visual-state-manager",
            Title = "Visual states",
            Description = "Change how a control looks in each state, from Normal and Focused to your own custom states.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.CursorHover24,
            PageType = typeof(VisualStateManagerPage),
            Keywords = ["VisualStateManager", "VSM", "VisualState", "VisualStateGroup", "CommonStates", "Normal", "Disabled", "Focused", "PointerOver", "Pressed", "Selected", "GoToState", "StateTrigger", "AdaptiveTrigger"],
            RelatedIds = ["triggers", "styles-and-resources", "entry", "button", "collection-view"],
            DocsUrl = new(DocsRoot + "user-interface/visual-states"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.visualstatemanager"),
            SourceUrl = new(SourceRoot + "Fundamentals/VisualStateManager"),
        },
        new()
        {
            Id = "triggers",
            Title = "Triggers",
            Description = "Change properties declaratively when a property, binding or event changes, or when a state condition holds.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.FlashFlow24,
            PageType = typeof(TriggersPage),
            Keywords = ["Trigger", "DataTrigger", "EventTrigger", "TriggerAction", "MultiTrigger", "BindingCondition", "StateTrigger", "AdaptiveTrigger", "CompareStateTrigger", "DeviceStateTrigger", "OrientationStateTrigger", "declarative"],
            RelatedIds = ["visual-state-manager", "behaviors", "value-converters", "entry", "button"],
            DocsUrl = new(DocsRoot + "fundamentals/triggers"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.trigger"),
            SourceUrl = new(SourceRoot + "Fundamentals/Triggers"),
        },
        new()
        {
            Id = "behaviors",
            Title = "Behaviors",
            Description = "Add reusable functionality to controls without subclassing, with Behavior<T> and attached behaviors.",
            CategoryId = CategoryIds.Fundamentals,
            IconGlyph = FluentIconsRegular.PuzzlePiece24,
            PageType = typeof(BehaviorsPage),
            Keywords = ["Behavior", "Behavior<T>", "attached behavior", "attached property", "OnAttachedTo", "validation", "EventToCommand", "command", "reuse"],
            RelatedIds = ["triggers", "mvvm-commands", "entry"],
            DocsUrl = new(DocsRoot + "fundamentals/behaviors"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.behavior-1"),
            SourceUrl = new(SourceRoot + "Fundamentals/Behaviors"),
        },
    ];
}
