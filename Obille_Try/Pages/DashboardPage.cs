using Obille_Try.Data;

namespace Obille_Try.Pages {
  // The dashboard: a column of the page's header, the three figures and the
  // two cards, with the kit's gap between them. The column's content edge
  // sits 6 px inside the page padding, the room the tiles' card shadows take.
  public partial class DashboardPage : UI.Page {
    public DashboardPage() {
      InitializeComponent();
      lowStockTile.Click += (_, _) => LowStockOpened?.Invoke(this, EventArgs.Empty);
      ShowStock(Designing ? SampleData.Materials : ReadMaterials());

      toolsTile.Value = SampleData.ToolsOverdue.ToString();
      toolsTile.Change = SampleData.ToolsOverdue - SampleData.ToolsOverdueLastWeek;

      pageHeader.Description = $"Malolos yard, week of {SampleData.Weeks[^1]}";

      flowsCard.Note = $"{SampleData.Weeks[0]} to {SampleData.Weeks[^1]}";
      flowsCard.Labels = SampleData.Weeks;
      flowsCard.Received = SampleData.Received;
      flowsCard.Issued = SampleData.Issued;
    }

    // The low stock figure opens the materials list.
    public event EventHandler? LowStockOpened;

    // Reads the Materials table again after a change.
    public void Reload() => ShowStock(ReadMaterials());

    static IReadOnlyList<MaterialRow> ReadMaterials() {
      try {
        return Inventory.All();
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
        return Array.Empty<MaterialRow>();
      }
    }

    // Stock value and low stock come from the table, so they agree with
    // the list the low stock tile opens.
    void ShowStock(IReadOnlyList<MaterialRow> materials) {
      var byWeek = SampleData.StockValueByWeek(materials);
      var now = byWeek[^1];
      stockValueTile.Value = $"₱{now / 1_000_000:0.00}M";
      stockValueTile.Trend = byWeek;
      stockValueTile.Change = byWeek[^2] == 0 ? 0 : Math.Round((now - byWeek[^2]) / byWeek[^2] * 100, 1);

      var low = materials.Count(m => m.IsLow);
      lowStockTile.Value = low.ToString();
      lowStockTile.Change = low - SampleData.LowStockLastWeek;
    }
  }
}
