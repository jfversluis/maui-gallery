using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class EmailAndSmsPage : SamplePage
{
    public EmailAndSmsPage(SamplePageServices services, EmailViewModel email, SmsViewModel sms)
        : base(services)
    {
        InitializeComponent();

        EmailSample.BindingContext = email;
        EmailOptions.BindingContext = email;
        EmailExample.NativeTarget = EmailButton;
        email.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(EmailViewModel.IsHtml):
                    EmailFormatSubstitution.Value = email.IsHtml ? "EmailBodyFormat.Html" : "EmailBodyFormat.PlainText";
                    break;
                case nameof(EmailViewModel.AttachLog):
                    EmailAttachSubstitution.Value = email.AttachLog ? "new EmailAttachment(Path.Combine(FileSystem.Current.CacheDirectory, \"log.txt\"))" : string.Empty;
                    break;
                case nameof(EmailViewModel.Result):
                    EmailExample.Output = email.Result;
                    break;
            }
        };

        SmsSample.BindingContext = sms;
        SmsExample.NativeTarget = SmsButton;
        sms.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SmsViewModel.Result))
            {
                SmsExample.Output = sms.Result;
            }
        };
    }
}
