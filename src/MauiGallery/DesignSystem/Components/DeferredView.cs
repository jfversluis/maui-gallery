using MauiGallery.Services;

namespace MauiGallery.DesignSystem.Components;

/// <summary>
/// Creates its content from a template once the page is on screen, a frame later (see <see cref="DeferredWork"/>),
/// so content further down a page doesn't delay its first frame. The content inherits the binding context.
/// </summary>
/// <example>
/// <code><![CDATA[
/// <DeferredView>
///     <DataTemplate x:DataType="local:HomeViewModel">
///         <VerticalStackLayout>...</VerticalStackLayout>
///     </DataTemplate>
/// </DeferredView>
/// ]]></code>
/// </example>
[ContentProperty(nameof(ContentTemplate))]
public sealed class DeferredView : ContentView
{
    public static readonly BindableProperty ContentTemplateProperty =
        BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(DeferredView));

    private bool _isQueued;

    public DeferredView() => Loaded += (_, _) => QueueCreateContent();

    public DataTemplate? ContentTemplate
    {
        get => (DataTemplate?)GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    private void QueueCreateContent()
    {
        if (Content is not null || _isQueued)
        {
            return;
        }

        _isQueued = true;
        DeferredWork.Enqueue(Dispatcher, () =>
        {
            _isQueued = false;
            if (!IsLoaded || Content is not null || ContentTemplate?.CreateContent() is not View content)
            {
                return false;
            }

            Content = content;
            return true;
        });
    }
}
