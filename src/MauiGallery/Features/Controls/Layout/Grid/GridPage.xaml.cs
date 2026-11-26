using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class GridPage : SamplePage
{
    private static readonly ColumnDefinitionCollectionTypeConverter ColumnsConverter = new();
    private static readonly RowDefinitionCollectionTypeConverter RowsConverter = new();

    public GridPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        DefinitionsExample.NativeTarget = DefinitionsGrid;
        FormExample.NativeTarget = FormGrid;
        LayeringExample.NativeTarget = LayeringGrid;
        OnDefinitionsChanged(this, EventArgs.Empty);
        OnPlacementChanged(this, EventArgs.Empty);
    }

    private void OnDefinitionsChanged(object? sender, EventArgs e)
    {
        if (DefinitionsColumns.SelectedItem is not string columns || DefinitionsRows.SelectedItem is not string rows)
        {
            return;
        }

        // XAML parses "Auto,*,80" with these type converters; in code they turn the chosen preset into definitions.
        DefinitionsGrid.ColumnDefinitions = (ColumnDefinitionCollection)ColumnsConverter.ConvertFromInvariantString(columns)!;
        DefinitionsGrid.RowDefinitions = (RowDefinitionCollection)RowsConverter.ConvertFromInvariantString(rows)!;
        ColumnDefinitionsCode.Value = ToCSharp(columns);
        RowDefinitionsCode.Value = ToCSharp(rows);
    }

    private void OnPlacementChanged(object? sender, EventArgs e)
    {
        Grid.SetRow(PlacedTile, (int)PlacementRow.Value);
        Grid.SetColumn(PlacedTile, (int)PlacementColumn.Value);
        Grid.SetRowSpan(PlacedTile, (int)PlacementRowSpan.Value);
        Grid.SetColumnSpan(PlacedTile, (int)PlacementColumnSpan.Value);
        PlacedTileText.Text = $"Row {PlacementRow.Value}, Column {PlacementColumn.Value}";
    }

    private void OnFormSendClicked(object? sender, EventArgs e) =>
        FormExample.Output = string.IsNullOrWhiteSpace(FormName.Text) ? "Send clicked" : $"Send clicked by {FormName.Text}";

    private void OnFormCancelClicked(object? sender, EventArgs e) => FormExample.Output = "Cancel clicked";

    // "Auto,*,80" becomes "[new(GridLength.Auto), new(GridLength.Star), new(80)]", the C# equivalent.
    private static string ToCSharp(string definitions) =>
        "[" + string.Join(", ", definitions.Split(',').Select(length => length switch
        {
            "Auto" => "new(GridLength.Auto)",
            "*" => "new(GridLength.Star)",
            _ when length.EndsWith('*') => $"new(new GridLength({length.TrimEnd('*')}, GridUnitType.Star))",
            _ => $"new({length})",
        })) + "]";
}
