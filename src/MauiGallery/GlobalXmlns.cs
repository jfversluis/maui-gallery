// Maps the app's XAML-facing namespaces into the .NET 11 global XAML namespace, so XAML files can use these
// types without xmlns declarations, just like built-in MAUI controls.
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiGallery.DesignSystem.Components")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiGallery.DesignSystem.Converters")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiGallery.DesignSystem.Layouts")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiGallery.Features.Samples")]
