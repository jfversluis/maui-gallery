using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.Pickers;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] PickersItems() =>
    [
        new()
        {
            Id = "picker",
            Title = "Picker",
            Description = "Lets people choose one item from a list, such as a size, a country or a category.",
            CategoryId = CategoryIds.Pickers,
            IconGlyph = FluentIconsRegular.ChevronUpDown24,
            PageType = typeof(PickerPage),
            Keywords = ["dropdown", "drop-down", "combo box", "combobox", "select", "list", "spinner", "choose"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIPickerView"),
                new(GalleryPlatforms.Android, "AlertDialog"),
                new(GalleryPlatforms.MacCatalyst, "UIPickerView"),
                new(GalleryPlatforms.Windows, "ComboBox"),
            ],
            RelatedIds = ["date-picker", "time-picker"],
            DocsUrl = new(DocsRoot + "user-interface/controls/picker"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.picker"),
            SourceUrl = new(SourceRoot + "Controls/Pickers/Picker"),
        },
        new()
        {
            Id = "date-picker",
            Title = "DatePicker",
            Description = "Lets people choose a date with the platform's calendar or wheel, within an optional range.",
            CategoryId = CategoryIds.Pickers,
            IconGlyph = FluentIconsRegular.CalendarMonth24,
            PageType = typeof(DatePickerPage),
            Keywords = ["date", "calendar", "day", "birthday", "booking", "range", "nullable", "IsOpen"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIDatePicker"),
                new(GalleryPlatforms.Android, "DatePickerDialog"),
                new(GalleryPlatforms.MacCatalyst, "UIDatePicker"),
                new(GalleryPlatforms.Windows, "CalendarDatePicker"),
            ],
            RelatedIds = ["time-picker", "picker"],
            DocsUrl = new(DocsRoot + "user-interface/controls/datepicker"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.datepicker"),
            SourceUrl = new(SourceRoot + "Controls/Pickers/DatePicker"),
            IsUpdated = true,
        },
        new()
        {
            Id = "time-picker",
            Title = "TimePicker",
            Description = "Lets people choose a time of day, in 12 or 24-hour format.",
            CategoryId = CategoryIds.Pickers,
            IconGlyph = FluentIconsRegular.Clock24,
            PageType = typeof(TimePickerPage),
            Keywords = ["time", "clock", "hour", "minute", "alarm", "reminder", "24-hour", "nullable", "IsOpen"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIDatePicker"),
                new(GalleryPlatforms.Android, "TimePickerDialog"),
                new(GalleryPlatforms.MacCatalyst, "UIDatePicker"),
                new(GalleryPlatforms.Windows, "TimePicker"),
            ],
            RelatedIds = ["date-picker", "picker"],
            DocsUrl = new(DocsRoot + "user-interface/controls/timepicker"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.timepicker"),
            SourceUrl = new(SourceRoot + "Controls/Pickers/TimePicker"),
            IsUpdated = true,
        },
    ];
}
