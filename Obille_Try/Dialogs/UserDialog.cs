using System.Text.RegularExpressions;
using Kit;
using Obille_Try.Data;

namespace Obille_Try.Dialogs {
  // New user, or Edit user when opened from a row. Save checks every field
  // and says under each what is wrong; the username is checked against the
  // table once the rest is right. Editing, the password fields are left
  // blank to keep the account's password. The signed-in account cannot
  // switch itself off, and the last active Admin cannot stop being one.
  public partial class UserDialog : FieldsDialog {
    static readonly Regex UsernameChars = new("^[A-Za-z0-9._-]+$");

    readonly UserRow? editing;

    public UserDialog() : this(null) {
    }

    public UserDialog(UserRow? user) {
      InitializeComponent();
      editing = user;
      roleBox.Options = Accounts.Roles;
      roleBox.SelectedOption = "User";

      if (user != null) {
        Title = $"Edit {user.FullName}";
        saveButton.Text = "Save changes";
        fullNameBox.Text = user.FullName;
        usernameBox.Text = user.Username;
        roleBox.SelectedOption = user.Role;
        activeBox.Checked = user.IsActive;
        passwordField.Label = "New password";
        passwordField.Optional = true;
        confirmField.Optional = true;
        passwordField.Help = "Leave both blank to keep the current one.";
        if (Accounts.Current?.Id == user.Id) {
          activeBox.Enabled = false;
          statusField.Help = "You are signed in with this account, so it stays active.";
        }
      }
      saveButton.Width = saveButton.GetPreferredSize(Size.Empty).Width;

      fullNameBox.TextChanged += (_, _) => ClearError(fullNameField);
      usernameBox.TextChanged += (_, _) => ClearError(usernameField);
      roleBox.SelectedIndexChanged += (_, _) => ClearError(roleField);
      passwordBox.TextChanged += (_, _) => ClearError(passwordField);
      confirmBox.TextChanged += (_, _) => ClearError(confirmField);
      activeBox.CheckedChanged += (_, _) => ClearError(statusField);
      FitHeight();
    }

    // The account as saved; null until Save succeeds.
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public UserRow? Saved { get; private set; }

    protected override TableLayoutPanel FieldTable => fields;

    protected override Field[] Fields => new[] { fullNameField, usernameField, roleField, passwordField, confirmField, statusField };

    void saveButton_Click(object? sender, EventArgs e) {
      var fullName = fullNameBox.Text.Trim();
      var username = usernameBox.Text.Trim();
      var password = passwordBox.Text;
      var confirm = confirmBox.Text;
      var keepPassword = editing != null && password.Length == 0 && confirm.Length == 0;
      var role = roleBox.SelectedOption ?? "";
      var active = activeBox.Checked;

      fullNameField.Error =
          fullName.Length == 0 ? "Enter the person's full name." :
          fullName.Length > 100 ? "Use 100 characters or fewer." : "";
      usernameField.Error =
          username.Length == 0 ? "Choose a username." :
          username.Length > 50 ? "Use 50 characters or fewer." :
          !UsernameChars.IsMatch(username) ? "Use letters, numbers, dots, dashes or underscores, with no spaces." : "";
      roleField.Error = role.Length == 0 ? "Choose a role." : "";
      passwordField.Error = keepPassword ? "" :
          password.Length < Accounts.MinPasswordLength ? $"Use at least {Accounts.MinPasswordLength} characters." : "";
      confirmField.Error = keepPassword ? "" :
          confirm.Length == 0 ? "Type the password again." :
          confirm != password ? "This does not match the password above." : "";
      statusField.Error = "";

      if (!HasErrors()) {
        try {
          if (Accounts.UsernameTaken(username, editing?.Id ?? 0))
            usernameField.Error = "That username is taken. Choose another.";
          else if (editing != null && Accounts.AdminRuleBroken(editing.Id, role, active) is { } rule)
            (active ? roleField : statusField).Error = $"{rule} Make another account an Admin first.";
        } catch (Exception ex) when (Data.Database.Failed(ex)) {
          ShowDatabaseProblem(ex);
          return;
        }
      }
      if (HasErrors()) {
        ShowErrorSummary(editing == null ? "add the user" : "save the changes");
        FocusFirstError();
        return;
      }

      Cursor = Cursors.WaitCursor;
      try {
        if (editing == null) {
          Saved = Accounts.Create(fullName, username, password, role, active);
        } else {
          var changed = editing with { FullName = fullName, Username = username, Role = role, IsActive = active };
          Accounts.Update(changed, keepPassword ? null : password);
          Saved = changed;
        }
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
        ShowDatabaseProblem(ex);
        return;
      } finally {
        Cursor = Cursors.Default;
      }
      DialogResult = DialogResult.OK;
    }

    protected override void OnShown(EventArgs e) {
      base.OnShown(e);
      fullNameBox.TextBox.Focus();
    }
  }
}
