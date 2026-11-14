using Kit;

namespace Obille_Try.Dialogs {
  // A yes or no question with a title, a line under it and a named action:
  // "Delete Maria Santos?", Delete user. Cancel holds the focus, so Enter
  // alone changes nothing.
  public partial class ConfirmDialog : KitDialog {
    public ConfirmDialog() {
      InitializeComponent();
    }

    public ConfirmDialog(string title, string description, string action, bool danger) : this() {
      Title = title;
      Description = description;
      actionButton.Text = action;
      actionButton.Variant = danger ? ButtonVariant.Danger : ButtonVariant.Primary;
      // As Delete material's button has it.
      if (danger) actionButton.Icon = "trash-2";
      actionButton.Width = actionButton.GetPreferredSize(Size.Empty).Width;
      ClientSize = new Size(ClientSize.Width, HeaderHeight + LogicalToDeviceUnits(8) + footer.Height);
    }

    protected override void OnShown(EventArgs e) {
      base.OnShown(e);
      cancelButton.Focus();
    }
  }
}
