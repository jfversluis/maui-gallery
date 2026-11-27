using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class DataBindingPage : SamplePage
{
    private readonly BindingModesViewModel _modes = new();

    public DataBindingPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        FormatExample.NativeTarget = FormatSlider;
        FallbackExample.NativeTarget = FallbackNickname;
        CompiledExample.NativeTarget = CompiledEntry;

        // Applying a OneWayToSource binding writes the entry's empty text to the source. Restore the starting name
        // afterwards, so the other rows begin from it and that entry shows it never reads the source.
        var initialName = _modes.Name;
        ModesSample.BindingContext = _modes;
        _modes.Name = initialName;
        _modes.PropertyChanged += (_, e) => ModesExample.Output = $"PropertyChanged: Name = \"{_modes.Name}\"";
        ModesExample.NativeTarget = ModesToSourceEntry;

        FallbackSource.SelectedIndex = 0;

        CompiledSample.BindingContext = new GreetingViewModel();
        ViewToViewExample.NativeTarget = ViewToViewSlider;
        ExpressionsSample.BindingContext = new TicketOrderViewModel();
        ExpressionsExample.NativeTarget = ExpressionsStepper;
    }

    private void OnFallbackSourceChanged(object? sender, EventArgs e)
    {
        (FallbackSample.BindingContext, FallbackExample.Output) = FallbackSource.SelectedIndex switch
        {
            1 => (new Teammate("Margaret", null), "Nickname is null: TargetNullValue is shown"),
            2 => ((Teammate?)null, "BindingContext is null, so there's nothing to bind to: FallbackValue is shown"),
            _ => (new Teammate("Margaret", "Maggie"), "Nickname = \"Maggie\""),
        };
    }
}
