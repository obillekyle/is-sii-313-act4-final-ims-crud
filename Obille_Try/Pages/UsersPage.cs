using System.ComponentModel;
using Kit;
using Obille_Try.Data;
using Obille_Try.Dialogs;

namespace Obille_Try.Pages {
  // User management: every account in the Users table, with New user,
  // Edit user, Deactivate (or Activate) and Delete user. The signed-in
  // account cannot be deactivated or deleted from here, and the last active
  // Admin cannot be demoted, deactivated or deleted; both are refused with
  // the reason, before anything is written.
  public partial class UsersPage : UI.Page {
    List<UserRow> users = new();
    readonly ContextMenuStrip rowMenu = new();
    readonly ToolStripMenuItem toggleItem;
    UserRow? menuUser;

    public UsersPage() {
      InitializeComponent();

      var px = LogicalToDeviceUnits(16);
      var edit = new ToolStripMenuItem("Edit user", Icons.ToBitmap("pencil", px, KitTheme.Palette.Ink)) {
        ShortcutKeyDisplayString = "Enter",
      };
      edit.Click += (_, _) => { if (menuUser != null) OpenEdit(menuUser); };
      toggleItem = new ToolStripMenuItem("Deactivate", Icons.ToBitmap("lock", px, KitTheme.Palette.Ink));
      toggleItem.Click += (_, _) => { if (menuUser != null) ToggleActive(menuUser); };
      var delete = new ToolStripMenuItem("Delete user", Icons.ToBitmap("trash-2", px, KitTheme.Palette.Bad)) {
        ForeColor = KitTheme.Palette.Bad,
        ShortcutKeyDisplayString = "Del",
      };
      delete.Click += (_, _) => { if (menuUser != null) ConfirmDelete(menuUser); };
      rowMenu.Items.AddRange(new ToolStripItem[] { edit, toggleItem, new ToolStripSeparator(), delete });
      KitMenu.Style(rowMenu);

      userGrid.RowMenuRequested += (_, e) => {
        menuUser = e.User;
        var on = e.User.IsActive;
        toggleItem.Text = on ? "Deactivate" : "Activate";
        toggleItem.Image = Icons.ToBitmap(on ? "lock" : "lock-open", px, KitTheme.Palette.Ink);
        rowMenu.Show(e.Screen);
      };
      userGrid.RowOpened += (_, u) => OpenEdit(u);
      userGrid.KeyDown += (_, e) => {
        if (e.KeyCode != Keys.Delete || userGrid.RowAt(userGrid.CurrentCell?.RowIndex ?? -1) is not { } u) return;
        ConfirmDelete(u);
        e.Handled = true;
      };
      newButton.Click += (_, _) => OpenNew();

      if (!Designing) Reload();
    }

    // Raised when the signed-in account's own name or role changed, so the
    // sidebar can show the new ones.
    public event EventHandler? CurrentAccountChanged;

    public void Reload() {
      try {
        users = Accounts.List();
        userGrid.EmptyText = "No accounts yet.";
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
        users = new();
        userGrid.EmptyText = $"The accounts database did not open. {ex.Message}";
      }
      userGrid.ShowRows(users);
      var active = users.Count(u => u.IsActive);
      pageHeader.Description = users.Count == 1
          ? $"1 account, {active} active"
          : $"{users.Count} accounts, {active} active";
    }

    public void OpenNew() {
      using var dialog = new UserDialog();
      if (FindForm() is not { } owner || dialog.ShowOver(owner) != DialogResult.OK || dialog.Saved is not { } saved) return;
      Reload();
      toaster.Show($"Added {saved.FullName}", Tone.Success);
    }

    void OpenEdit(UserRow user) {
      using var dialog = new UserDialog(user);
      if (FindForm() is not { } owner || dialog.ShowOver(owner) != DialogResult.OK || dialog.Saved is not { } saved) return;
      Reload();
      toaster.Show($"Saved {saved.FullName}", Tone.Success);
      if (Accounts.Current?.Id == saved.Id) CurrentAccountChanged?.Invoke(this, EventArgs.Empty);
    }

    void ToggleActive(UserRow user) {
      var on = !user.IsActive;
      if (!on && Accounts.Current?.Id == user.Id) {
        toaster.Show("You are signed in with this account, so it stays active.", Tone.Warning);
        return;
      }
      try {
        if (Accounts.AdminRuleBroken(user.Id, user.Role, on) is { } rule) {
          toaster.Show($"{rule} Make another account an Admin first.", Tone.Warning);
          return;
        }
        Accounts.Update(user with { IsActive = on }, null);
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
        toaster.Show($"Nothing was changed. {ex.Message}", Tone.Danger);
        return;
      }
      Reload();
      toaster.Show(on ? $"{user.FullName} can sign in again" : $"{user.FullName} can no longer sign in", Tone.Success);
    }

    void ConfirmDelete(UserRow user) {
      if (Accounts.Current?.Id == user.Id) {
        toaster.Show("You are signed in with this account, so it can't be deleted here.", Tone.Warning);
        return;
      }
      try {
        if (Accounts.AdminRuleBroken(user.Id, "User", false) is { } rule) {
          toaster.Show($"{rule} Make another account an Admin first.", Tone.Warning);
          return;
        }
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
        toaster.Show($"Nothing was changed. {ex.Message}", Tone.Danger);
        return;
      }

      using var dialog = new ConfirmDialog($"Delete {user.FullName}?",
          user.IsActive
              ? $"{user.FullName} ({user.Username}) can no longer sign in, and the account leaves the list. This can't be undone."
              : $"{user.FullName} ({user.Username}) leaves the list. This can't be undone.",
          "Delete user", danger: true);
      if (FindForm() is not { } owner || dialog.ShowOver(owner) != DialogResult.OK) return;
      try {
        Accounts.Delete(user.Id);
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
        toaster.Show($"Nothing was changed. {ex.Message}", Tone.Danger);
        return;
      }
      Reload();
      toaster.Show($"Deleted {user.FullName}", Tone.Success);
    }
  }
}
