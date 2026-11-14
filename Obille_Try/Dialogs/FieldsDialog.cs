using Kit;

namespace Obille_Try.Dialogs {
  // A dialog of Fields above a footer, as New material and New user are:
  // it is as tall as its header, its fields and its footer, grows when a
  // field shows an error and shrinks when the error goes, and puts the
  // focus in the first field with one. After a refused save, a line above
  // the fields counts the fields left to fix, so an error under a field
  // the reader has not looked at is not missed. A subclass names its field
  // table and its fields in order.
  public class FieldsDialog : KitDialog {
    protected virtual TableLayoutPanel? FieldTable => null;

    protected virtual Field[] Fields => Array.Empty<Field>();

    protected bool HasErrors() => Fields.Any(f => f.Error.Length > 0);

    Panel? summaryRow;
    Alert? summary;
    string summaryGoal = "";

    // Shows the count above the fields: "Fix 3 fields to add the material."
    // It follows the errors from then on and goes with the last of them.
    protected void ShowErrorSummary(string goal) {
      summaryGoal = goal;
      if (summaryRow == null) {
        summary = new Alert { Tone = Tone.Danger, Dock = DockStyle.Top };
        summaryRow = new Panel { Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, LogicalToDeviceUnits(12)) };
        summaryRow.Controls.Add(summary);
        // The last control added docks first, so it sits above the fields.
        Controls.Add(summaryRow);
      }
      FitHeight();
    }

    void UpdateSummary() {
      if (summaryRow == null || summary == null) return;
      var n = Fields.Count(f => f.Error.Length > 0);
      summary.Text = n == 1 ? $"Fix 1 field to {summaryGoal}." : $"Fix {n} fields to {summaryGoal}.";
      summaryShown = n > 0;
      summaryRow.Visible = summaryShown;
      summaryRow.Height = summary.Height + summaryRow.Padding.Bottom;
    }

    // Kept apart from summaryRow.Visible, which reads false for a child of
    // a dialog not yet shown.
    bool summaryShown;

    protected void FocusFirstError() {
      if (Fields.FirstOrDefault(f => f.Error.Length > 0)?.Controls.OfType<Control>().FirstOrDefault() is not { } control) return;
      if (control is KitInputBase input) input.TextBox.Focus();
      else control.Focus();
    }

    // A field's error goes as soon as the field changes.
    protected void ClearError(Field field) {
      if (field.Error.Length == 0) return;
      field.Error = "";
      FitHeight();
    }

    protected void ShowDatabaseProblem(Exception ex) =>
        MessageBox.Show(this, $"Nothing was saved. {ex.Message}", "The database did not open",
            MessageBoxButtons.OK, MessageBoxIcon.Error);

    // The fields' height depends on the hints and errors that wrap at this
    // width. A shown dialog that grows is painted again 30 ms later: the
    // composited window drops a paint asked for in the same pass.
    protected void FitHeight() {
      if (FieldTable is not { } table || Controls.OfType<DialogFooter>().FirstOrDefault() is not { } footer) return;
      UpdateSummary();
      foreach (var f in Fields) f.PerformLayout();
      table.PerformLayout();
      var above = summaryShown && summaryRow != null ? summaryRow.Height : 0;
      var height = HeaderHeight + above + table.GetPreferredSize(new Size(table.Width, 0)).Height + LogicalToDeviceUnits(16) + footer.Height;
      if (ClientSize.Height == height) return;
      ClientSize = new Size(ClientSize.Width, height);
      if (!Visible) return;
      var timer = new System.Windows.Forms.Timer { Interval = 30 };
      timer.Tick += (_, _) => {
        timer.Dispose();
        if (!IsDisposed) Invalidate(true);
      };
      timer.Start();
    }
  }
}
