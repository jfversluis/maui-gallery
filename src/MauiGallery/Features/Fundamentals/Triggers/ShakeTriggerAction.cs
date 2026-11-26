namespace MauiGallery.Features.Fundamentals;

/// <summary>Shakes the element sideways, for an EventTrigger to run when something goes wrong.</summary>
public sealed class ShakeTriggerAction : TriggerAction<VisualElement>
{
    public double Distance { get; set; } = 12;

    protected override void Invoke(VisualElement sender)
    {
        ArgumentNullException.ThrowIfNull(sender);

        double[] offsets = [Distance, -Distance, Distance * 0.6, -Distance * 0.6, Distance * 0.3, 0];
        var shake = new Animation();
        var from = 0d;
        for (var i = 0; i < offsets.Length; i++)
        {
            var to = offsets[i];
            shake.Add((double)i / offsets.Length, (double)(i + 1) / offsets.Length, new Animation(x => sender.TranslationX = x, from, to));
            from = to;
        }

        shake.Commit(sender, "Shake", length: 400);
    }
}
