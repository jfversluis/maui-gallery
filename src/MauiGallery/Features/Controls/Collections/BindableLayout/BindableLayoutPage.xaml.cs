using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Collections;

public sealed partial class BindableLayoutPage : SamplePage
{
    public BindableLayoutPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        var tags = new TagListViewModel();
        tags.TagsChanged += (_, message) => TagsExample.Output = message;
        TagsSample.BindingContext = tags;
        TagsExample.NativeTarget = TagsLayout;

        var tasks = new TaskListViewModel();
        tasks.TasksChanged += (_, message) => TasksExample.Output = message;
        TasksSample.BindingContext = tasks;
        TasksRestore.Command = tasks.RestoreCommand;

        ChatSample.BindingContext = new ChatViewModel();
        ChatExample.NativeTarget = ChatLayout;
    }
}
