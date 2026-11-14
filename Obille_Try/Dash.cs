using Kit;

namespace Obille_Try {
  // The main window. Ctrl+1 to Ctrl+7 open the pages in the navigation's
  // order, Ctrl+N a new material, Ctrl+F the search.
  public partial class Dash : Form {
    sealed record PageInfo(string Key, string Title, string Icon);

    readonly List<PageInfo> pages;

    public Dash() {
      InitializeComponent();
      ToolStripManager.Renderer = new KitMenuRenderer();
      Icon = (Icon?)new System.ComponentModel.ComponentResourceManager(typeof(Login)).GetObject("$this.Icon");

      // The pages in the navigation's order, read from its items so the
      // two cannot disagree.
      pages = sidebar.Items.Select(i => new PageInfo(i.Key, i.Text, i.Icon)).ToList();
      sidebar.SelectedIndexChanged += (_, _) => {
        if (sidebar.SelectedIndex >= 0) Show(pages[sidebar.SelectedIndex].Key);
      };

      searchButton.Click += (_, _) => FindMaterial();
      locationSelect.Options = Data.SampleData.Locations;
      locationSelect.SelectedIndex = 0;
      bellButton.DropDown = BellMenu();
      sidebar.AccountMenu = UserMenu();
      // The designer's sample person stays when nobody is signed in.
      if (Data.Accounts.Current is { } account) {
        sidebar.PersonName = account.FullName;
        sidebar.PersonDetail = account.Role;
      }

      // The first New material otherwise waits most of a second for .NET
      // to compile the dialog's code (630 to 1500 ms measured, 110 to
      // 150 after); the page's kept dialog is opened off screen once
      // the window shows and the app is idle, and hidden again.
      Shown += (_, _) => KitDialog.Warm(() => materialsPage.MaterialDialog, keep: true);

      dashboardPage.LowStockOpened += (_, _) => { Go("materials"); materialsPage.ShowLowStock(); };
      placeholderPage.DashboardRequested += (_, _) => Go("dashboard");

      // A saved material changes the dashboard's figures and the low stock
      // list under the bell. The event is static, so the window lets go of
      // it when it closes and a sign-in after a sign out starts clean.
      EventHandler stockChanged = (_, _) => {
        dashboardPage.Reload();
        bellButton.DropDown = BellMenu();
      };
      Data.Inventory.Changed += stockChanged;
      FormClosed += (_, _) => Data.Inventory.Changed -= stockChanged;
      usersPage.CurrentAccountChanged += (_, _) => {
        if (Data.Accounts.Current is not { } me) return;
        sidebar.PersonName = me.FullName;
        sidebar.PersonDetail = me.Role;
      };

      Go("dashboard");
    }

    // Set when the window closes by Sign out, so the sign-in screen comes back
    // instead of the app ending.
    public bool SignedOut { get; private set; }

    // WS_EX_COMPOSITED: the window and its children paint off screen and
    // show together, so a page switch or a bar swap never shows half its
    // controls painted.
    protected override CreateParams CreateParams {
      get {
        var cp = base.CreateParams;
        cp.ExStyle |= 0x02000000;
        return cp;
      }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      switch (keyData) {
        case Keys.Control | Keys.N:
          Go("materials");
          materialsPage.OpenNew();
          return true;
        case Keys.Control | Keys.F:
          FindMaterial();
          return true;
        case var k when (k & Keys.Modifiers) == Keys.Control && (k & Keys.KeyCode) >= Keys.D1
            && (k & Keys.KeyCode) < Keys.D1 + pages.Count:
          Go(pages[(int)(k & Keys.KeyCode) - (int)Keys.D1].Key);
          return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    // Opens a page by its key; the navigation's selection follows, and its
    // change shows the page.
    void Go(string key) {
      var index = pages.FindIndex(p => p.Key == key);
      if (sidebar.SelectedIndex == index) Show(key);
      else sidebar.SelectedIndex = index;
    }

    void Show(string key) {
      var page = pages.First(p => p.Key == key);
      var view = key switch {
        "dashboard" => (Control)dashboardPage,
        "materials" => materialsPage,
        "users" => usersPage,
        _ => placeholderPage,
      };
      if (view == placeholderPage) placeholderPage.ShowFor(page.Title, page.Icon);
      foreach (Control c in content.Controls)
        if (c is UserControl) c.Visible = c == view;

      // The sketch's dashboard has no trail; Materials sits under Stock.
      breadcrumbs.Items = key switch {
        "dashboard" => Array.Empty<string>(),
        "materials" => new[] { "Stock", "Materials" },
        _ => new[] { page.Title },
      };
    }

    void FindMaterial() {
      Go("materials");
      materialsPage.FocusSearch();
    }

    void SignOut() {
      using var confirm = new Dialogs.SignOutDialog();
      if (confirm.ShowOver(this) != DialogResult.OK) return;
      SignedOut = true;
      Close();
    }

    ContextMenuStrip BellMenu() {
      var menu = new ContextMenuStrip();
      List<Data.MaterialRow> low;
      try {
        low = Data.Inventory.All().Where(r => r.IsLow).ToList();
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
        low = new();
      }
      var head = new ToolStripLabel($"{low.Count} low on stock at Malolos yard") {
        Font = KitTheme.Font(13, true),
        ForeColor = KitTheme.Palette.Ink2,
      };
      menu.Items.Add(head);
      foreach (var r in low) {
        var item = new ToolStripMenuItem($"{r.Name} is down to {r.Quantity(r.Stock)}",
            Icons.ToBitmap("triangle-alert", LogicalToDeviceUnits(16), KitTheme.Palette.Warn));
        item.Click += (_, _) => { Go("materials"); materialsPage.ShowLowStock(); };
        menu.Items.Add(item);
      }
      return menu;
    }

    ContextMenuStrip UserMenu() {
      var menu = new ContextMenuStrip();
      var ink = KitTheme.Palette.Ink;
      var px = LogicalToDeviceUnits(16);
      var about = new ToolStripMenuItem("About SS Construction Inventory", Icons.ToBitmap("info", px, ink));
      about.Click += (_, _) => MessageBox.Show(this,
          "SS Construction Inventory, version 1.1.\nMaterials and user accounts are saved in an Access database.",
          "About SS Construction Inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
      var signOut = new ToolStripMenuItem("Sign out", Icons.ToBitmap("log-out", px, ink));
      signOut.Click += (_, _) => SignOut();
      menu.Items.AddRange(new ToolStripItem[] { about, new ToolStripSeparator(), signOut });
      return menu;
    }
  }
}
