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
            IsFeatured = true,
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
    ];
}
