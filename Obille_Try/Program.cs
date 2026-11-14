namespace Obille_Try {
  internal static class Program {
    [STAThread]
    static void Main() {
      ApplicationConfiguration.Initialize();
      // A database from before the Materials table gets it here. When the
      // file does not open, sign in reports it, so nothing is said twice.
      try {
        Data.Inventory.EnsureTable();
      } catch (Exception ex) when (Data.Database.Failed(ex)) {
      }
      Application.Run(new Login());
    }
  }
}
