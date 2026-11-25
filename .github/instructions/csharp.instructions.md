---
applyTo: "**/*.cs"
---

# C# conventions

- File-scoped namespaces; `sealed` classes unless designed for inheritance.
- View models derive from `ObservableObject` and use `[ObservableProperty]` / `[RelayCommand]` source generators.
- Inject dependencies through constructors; register them in `MauiProgram.cs`.
- No `async void` except event handlers. Do not swallow exceptions.
- Logic without a MAUI dependency belongs in `MauiGallery.Core` and needs unit tests.
