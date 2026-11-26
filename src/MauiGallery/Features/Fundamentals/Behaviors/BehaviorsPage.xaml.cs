using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class BehaviorsPage : SamplePage
{
    public BehaviorsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        AttachedExample.NativeTarget = AttachedEntry;
        ValidationExample.NativeTarget = ValidationEntry;
        CommandExample.NativeTarget = CommandUsername;

        ValidationBehavior.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NumericValidationBehavior.IsValid))
            {
                ValidationExample.Output = $"NumericValidationBehavior.IsValid = {ValidationBehavior.IsValid}";
            }
        };

        var username = new UsernameViewModel();
        username.CheckAvailabilityCommand.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(username.CheckAvailabilityCommand.IsRunning) && username.CheckAvailabilityCommand.IsRunning)
            {
                CommandExample.Output = "Unfocused: the behavior ran CheckAvailabilityCommand";
            }
        };
        CommandSample.BindingContext = username;
    }
}
