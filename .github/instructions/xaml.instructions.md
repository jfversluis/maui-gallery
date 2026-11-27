---
applyTo: "**/*.xaml"
---

# XAML conventions

- Do not declare `xmlns` for MAUI types; .NET 11 implicit namespaces provide them. App namespaces are global
  through `GlobalXmlns.cs`, which also maps the Fluent icons (`{x:Static FluentIconsRegular.Add24}`); map any new app
  namespace there and keep type names unique across them. Only other third-party namespaces need a prefix. Snippets
  keep the `local:` and `icons:` prefixes a developer's own app would use.
- Set `x:DataType` on every page, view and `DataTemplate`.
- Bind view-model properties with .NET 11 C# expressions: `{Name}`, `{$'Total: {Total:C}'}`, `{!IsBusy}`. Write
  `{.Name}` when the page also has a member with that name (`Title`) or it matches a type (`Color`). Keep `{Binding}`
  for element-to-element bindings (`Source={x:Reference ...}`, expressions read named elements only once),
  `[RelayCommand]` commands and nullable reference properties (not supported by expressions in MAUI 11.0.x), and
  bindings that need `Mode`, `FallbackValue` or `TargetNullValue`.
- Chrome uses design-system resources only: `{StaticResource ...}` for tokens and keyed styles, never literal
  colors, font sizes or spacing values.
- Never add an implicit style for a core control. Samples on the stage must look like the platform default.
- Give interactive chrome elements `SemanticProperties.Description` (or a visible label) and a stable `AutomationId`.
