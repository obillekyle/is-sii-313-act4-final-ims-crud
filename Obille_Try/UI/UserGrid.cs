using Kit;
using Obille_Try.Data;

namespace Obille_Try.UI {
  public sealed class UserMenuEventArgs : EventArgs {
    public UserMenuEventArgs(UserRow user, Point screen) { User = user; Screen = screen; }
    public UserRow User { get; }
    public Point Screen { get; }
  }

  // The accounts table, on the kit's table. The person's cell holds two
  // lines, the full name over the username; Status is a pill with a dot,
  // green when active; Role and Created are the kit's own text cells; the
  // last cell is the row's menu button. Columns are known by their
  // DataPropertyName, as MaterialGrid's are.
  public class UserGrid : KitTable {
    public UserGrid() {
      AllowUserToDeleteRows = false;
      AllowUserToResizeColumns = false;
      StandardTab = true;
    }

    protected override int RowHeight => S(56);

    public event EventHandler<UserMenuEventArgs>? RowMenuRequested;
    public event EventHandler<UserRow>? RowOpened;

    public UserRow? RowAt(int index) =>
        index >= 0 && index < Rows.Count ? Rows[index].Tag as UserRow : null;

    string KeyOf(int column) => column >= 0 && column < Columns.Count ? Columns[column].DataPropertyName : "";

    int IndexOf(string key) {
      foreach (DataGridViewColumn c in Columns)
        if (c.DataPropertyName == key) return c.Index;
      throw new InvalidOperationException($"The users table has no column whose DataPropertyName is {key}.");
    }

    public void ShowRows(IEnumerable<UserRow> users) {
      Rows.Clear();
      foreach (var u in users) {
        var row = Rows[Rows.Add()];
        row.Tag = u;
        var you = Accounts.Current?.Id == u.Id ? " (you)" : "";
        row.Cells[IndexOf("Person")].Value = $"{u.FullName}{you}, {u.Username}";
        row.Cells[IndexOf("Role")].Value = u.Role;
        row.Cells[IndexOf("Status")].Value = u.IsActive ? "Active" : "Inactive";
        row.Cells[IndexOf("Created")].Value = u.CreatedAt.ToString("MMM d, yyyy");
      }
    }

    protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e) {
      base.OnCellMouseClick(e);
      if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
      if (e.Button == MouseButtons.Right || KeyOf(e.ColumnIndex) == "Actions") {
        var at = e.Button == MouseButtons.Right ? Cursor.Position : PointToScreen(MenuAnchor(e.RowIndex));
        OpenMenu(e.RowIndex, at);
      }
    }

    protected override void OnCellDoubleClick(DataGridViewCellEventArgs e) {
      base.OnCellDoubleClick(e);
      if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && KeyOf(e.ColumnIndex) != "Actions" && RowAt(e.RowIndex) is { } u)
        RowOpened?.Invoke(this, u);
    }

    protected override bool ProcessDialogKey(Keys keyData) {
      var row = CurrentCell?.RowIndex ?? -1;
      switch (keyData) {
        case Keys.Enter when RowAt(row) is { } u:
          RowOpened?.Invoke(this, u);
          return true;
        case Keys.Apps when row >= 0:
        case Keys.Shift | Keys.F10 when row >= 0:
          OpenMenu(row, PointToScreen(MenuAnchor(row)));
          return true;
      }
      return base.ProcessDialogKey(keyData);
    }

    void OpenMenu(int row, Point screen) {
      if (RowAt(row) is { } u) RowMenuRequested?.Invoke(this, new UserMenuEventArgs(u, screen));
    }

    Point MenuAnchor(int row) {
      var box = ActionBox(GetCellDisplayRectangle(IndexOf("Actions"), row, false));
      return new Point(box.X, box.Bottom + S(4));
    }

    Rectangle ActionBox(Rectangle cell) {
      var size = S(28);
      var (_, right) = PadFor(IndexOf("Actions"));
      return new Rectangle(cell.Right - right - size, cell.Y + (cell.Height - size) / 2, size, size);
    }

    protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e) {
      var name = KeyOf(e.ColumnIndex);
      if (e.Graphics == null || e.RowIndex < 0 || RowAt(e.RowIndex) is not { } u || name is not ("Person" or "Status" or "Actions")) {
        base.OnCellPainting(e);
        return;
      }
      var g = e.Graphics;
      var b = e.CellBounds;
      using (var fill = new SolidBrush(RowFill(e.RowIndex))) g.FillRectangle(fill, b);
      if (e.RowIndex != Rows.Count - 1) Rule(g, b);

      if (name == "Person") {
        var (left, right) = PadFor(e.ColumnIndex);
        var main = KitTheme.Font(14);
        var sub = KitTheme.Font(13);
        int h1 = main.Height, h2 = sub.Height;
        var top = b.Y + (b.Height - h1 - h2 - S(2)) / 2;
        var width = b.Width - left - right;
        var you = Accounts.Current?.Id == u.Id ? " (you)" : "";
        Painter.Text(g, u.FullName + you, main, new Rectangle(b.X + left, top, width, h1), P.Ink);
        Painter.Text(g, u.Username, sub, new Rectangle(b.X + left, top + h1 + S(2), width, h2), P.Ink2);
      } else if (name == "Status") {
        // A pill in the tone's soft fill with a dot before the word, so
        // the state does not rest on the word's color alone.
        var (ink, soft) = P.ToneColors(u.IsActive ? Tone.Success : Tone.Neutral);
        var font = KitTheme.Font(13);
        var text = u.IsActive ? "Active" : "Inactive";
        var (left, _) = PadFor(e.ColumnIndex);
        var dot = S(6);
        var pill = new Rectangle(b.X + left, b.Y + (b.Height - S(22)) / 2,
            S(8) + dot + S(6) + Painter.Measure(text, font).Width + S(10), S(22));
        Painter.Smooth(g);
        Painter.Fill(g, soft, pill, pill.Height / 2f);
        using (var brush = new SolidBrush(u.IsActive ? P.Ok : P.Ink3))
          g.FillEllipse(brush, pill.X + S(8), pill.Y + (pill.Height - dot) / 2f, dot, dot);
        Painter.Text(g, text, font, new Rectangle(pill.X + S(8) + dot + S(6), pill.Y, pill.Width, pill.Height), ink);
      } else {
        var box = ActionBox(b);
        Painter.Smooth(g);
        if (RowFill(e.RowIndex) != P.Bg) Painter.Fill(g, P.BgHover, box, S(KitTheme.Metrics.RadiusControl));
        Icons.Draw(g, "ellipsis", RectangleF.Inflate(box, -S(6), -S(6)), P.Ink2);
      }
      e.Handled = true;
    }
  }
}
