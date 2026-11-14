using System.Globalization;
using System.Text.RegularExpressions;
using Kit;
using Obille_Try.Data;

namespace Obille_Try.Dialogs {
  // New material, or Edit material when opened from a row. Add material
  // (or Save changes) checks every field, says under each what is wrong,
  // and saves to the Materials table only when all of them are right; the
  // dialog closes once the row is saved. Saved holds the row as stored.
  public partial class MaterialDialog : FieldsDialog {
    static readonly Regex CodeChars = new("^[A-Za-z0-9-]+$");

    static string CostLabel(string unitDisplay) =>
        $"Standard cost per {SampleData.Units.First(u => u.Display == unitDisplay).Singular}, in pesos";

    public MaterialDialog() {
      InitializeComponent();
      categoryBox.Options = SampleData.Categories;
      unitBox.Options = SampleData.Units.Select(u => u.Display).ToArray();
      supplierBox.Options = SampleData.Suppliers;
      // Codes are saved in capitals, so they are typed in capitals.
      codeBox.TextBox.CharacterCasing = CharacterCasing.Upper;
      // The sketch's new material: a steel item counted in pieces.
      categoryBox.SelectedOption = "Steel";
      unitBox.SelectedOption = "Pieces (pc)";
      costField.Label = CostLabel("Pieces (pc)");
      unitBox.SelectedIndexChanged += (_, _) => {
        if (unitBox.SelectedOption is { } unit) costField.Label = CostLabel(unit);
      };

      // A field's error goes as soon as the field changes.
      nameBox.TextChanged += (_, _) => ClearError(nameField);
      codeBox.TextChanged += (_, _) => ClearError(codeField);
      categoryBox.SelectedIndexChanged += (_, _) => ClearError(categoryField);
      unitBox.SelectedIndexChanged += (_, _) => ClearError(unitField);
      costBox.TextChanged += (_, _) => ClearError(costField);
      stockBox.ValueChanged += (_, _) => ClearError(stockField);
      reorderPointBox.ValueChanged += (_, _) => ClearError(reorderPointField);
      reorderQuantityBox.ValueChanged += (_, _) => ClearError(reorderQuantityField);
      binBox.TextChanged += (_, _) => ClearError(binField);
      FitHeight();

      // The page keeps one of these and opens it again, so what the
      // designer and the lines above set is kept to put back.
      var (title, action, category, unit) = (Title, addButton.Text, categoryBox.SelectedIndex, unitBox.SelectedIndex);
      clear = () => {
        editing = null;
        Saved = null;
        Title = title;
        addButton.Text = action;
        nameBox.Text = "";
        codeBox.Text = "";
        categoryBox.SelectedIndex = category;
        unitBox.SelectedIndex = unit;
        costBox.Text = "";
        stockBox.Value = null;
        reorderPointBox.Value = null;
        reorderQuantityBox.Value = null;
        supplierBox.SelectedIndex = -1;
        binBox.Text = "";
        foreach (var f in Fields) f.Error = "";
        FitButton();
        FitHeight();
      };
    }

    readonly Action clear;
    MaterialRow? editing;

    // The row as saved, with its ID; null until Add or Save succeeds.
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public MaterialRow? Saved { get; private set; }

    protected override TableLayoutPanel FieldTable => fields;

    protected override Field[] Fields => new[] {
      nameField, codeField, categoryField, unitField, costField, stockField,
      reorderPointField, reorderQuantityField, supplierField, binField,
    };

    // Back to a blank New material.
    public void Clear() => clear();

    public void LoadRow(MaterialRow row) {
      editing = row;
      Title = "Edit material";
      addButton.Text = "Save changes";
      nameBox.Text = row.Name;
      codeBox.Text = row.Code;
      categoryBox.SelectedOption = row.Category;
      unitBox.SelectedOption = SampleData.Units.FirstOrDefault(u => u.Abbr == row.Unit).Display;
      costBox.Text = row.Cost.ToString("N2");
      stockBox.Value = row.Stock;
      reorderPointBox.Value = row.ReorderPoint;
      reorderQuantityBox.Value = row.ReorderQuantity;
      supplierBox.SelectedOption = row.Supplier.Length > 0 ? row.Supplier : null;
      if (row.Supplier.Length == 0) supplierBox.SelectedIndex = -1;
      binBox.Text = row.Bin;
      foreach (var f in Fields) f.Error = "";
      FitButton();
      FitHeight();
    }

    void addButton_Click(object? sender, EventArgs e) {
      var name = nameBox.Text.Trim();
      var code = codeBox.Text.Trim().ToUpperInvariant();
      var bin = binBox.Text.Trim();
      var costText = costBox.Text.Trim().Replace("₱", "");
      var costOk = decimal.TryParse(costText, NumberStyles.Number, CultureInfo.InvariantCulture, out var cost);

      nameField.Error =
          name.Length == 0 ? "Enter the material's name." :
          name.Length > 150 ? "Use 150 characters or fewer." : "";
      codeField.Error =
          code.Length == 0 ? "Enter a code, such as STL-0302." :
          code.Length > 20 ? "Use 20 characters or fewer." :
          !CodeChars.IsMatch(code) ? "Use letters, numbers and dashes only." : "";
      categoryField.Error = categoryBox.SelectedIndex < 0 ? "Choose a category." : "";
      unitField.Error = unitBox.SelectedIndex < 0 ? "Choose what it is counted in." : "";
      costField.Error =
          costText.Length == 0 ? "Enter the cost, such as 265.00." :
          !costOk ? "Enter a number, such as 265.00." :
          cost < 0 ? "Enter zero or more." :
          cost > 10_000_000 ? "Enter 10,000,000 or less." : "";
      stockField.Error = stockBox.Value == null ? "Enter how many are in stock, or 0." : "";
      reorderPointField.Error = reorderPointBox.Value == null ? "Enter the reorder point, or 0." : "";
      reorderQuantityField.Error = reorderQuantityBox.Value == null ? "Enter how many to order, or 0." : "";
      binField.Error = bin.Length > 60 ? "Use 60 characters or fewer." : "";

      // The code is checked against the table only once the rest is right.
      if (!HasErrors()) {
        try {
          if (Inventory.CodeTaken(code, editing?.Id ?? 0))
            codeField.Error = $"{code} is already in the list. Use another code.";
        } catch (Exception ex) when (Data.Database.Failed(ex)) {
          ShowDatabaseProblem(ex);
          return;
        }
      }
      if (HasErrors()) {
        ShowErrorSummary(editing == null ? "add the material" : "save the changes");
        FocusFirstError();
        return;
      }

      var row = new MaterialRow(
          Name: name,
          Code: code,
          Bin: bin,
          Stock: (int)stockBox.Value!.Value,
          Unit: SampleData.Units.First(u => u.Display == unitBox.SelectedOption).Abbr,
          Cost: Math.Round(cost, 2),
          ReorderPoint: (int)reorderPointBox.Value!.Value,
          Category: categoryBox.SelectedOption!,
          Supplier: supplierBox.SelectedOption ?? "",
          ReorderQuantity: (int)reorderQuantityBox.Value!.Value,
          Id: editing?.Id ?? 0);

      Cursor = Cursors.WaitCursor;
      try {
        if (editing == null) {
          Saved = Inventory.Add(row);
        } else {
          Inventory.Update(row);
          Saved = row;
        }
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
        ShowDatabaseProblem(ex);
        return;
      } finally {
        Cursor = Cursors.Default;
      }
      DialogResult = DialogResult.OK;
    }

    // The button's words change between Add material and Save changes.
    void FitButton() => addButton.Width = addButton.GetPreferredSize(Size.Empty).Width;

    protected override void OnShown(EventArgs e) {
      base.OnShown(e);
      nameBox.TextBox.Focus();
    }
  }
}
