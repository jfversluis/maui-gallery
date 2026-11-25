---
applyTo: "**/*.xaml"
---

# XAML conventions

- Do not declare `xmlns` for MAUI types; .NET 11 implicit namespaces provide them. App namespaces are global
  through `GlobalXmlns.cs`. Only third-party namespaces (for example `IconFont.Maui.FluentIcons`) need a prefix.
- Set `x:DataType` on every page, view and `DataTemplate`.
- Chrome uses design-system resources only: `{StaticResource ...}` for tokens and keyed styles, never literal
  colors, font sizes or spacing values.
- Never add an implicit style for a core control. Samples on the stage must look like the platform default.
- Give interactive chrome elements `SemanticProperties.Description` (or a visible label) and a stable `AutomationId`.
