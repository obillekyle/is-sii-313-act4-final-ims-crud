using System.Data.OleDb;

namespace Obille_Try.Data {
  // The app's database: an Access file, Data\ims.accdb, copied beside the
  // exe at build. It is read through the Microsoft Access Database Engine's
  // OLE DB provider (ACE), which has to be installed with the same bitness
  // as the app runs at: 64-bit here, where an AnyCPU build runs as x64.
  // Two tables: Users (Accounts) and Materials (Inventory, which makes it
  // in a file that lacks it). tools\make-db.ps1 makes an empty file.
  public static class Database {
    // Replaceable so a check can run against a copy of the file.
    public static string FilePath { get; set; } = Path.Combine(AppContext.BaseDirectory, "Data", "ims.accdb");

    // A new, closed connection. The caller opens it inside a using, so it
    // is closed again however the query ends; ACE holds a connection's
    // writes until it closes, so a read on another connection opened
    // before that saw the row as it was.
    public static OleDbConnection GetConnection() =>
        new($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={FilePath};Persist Security Info=False");

    // What a missing engine, a missing file or a refused statement throws.
    public static bool Failed(Exception ex) => ex is OleDbException or InvalidOperationException;

    // Now, in whole seconds, so a row handed back after a write equals the
    // row read back later.
    public static DateTime Now() {
      var t = DateTime.Now;
      return t.AddTicks(-(t.Ticks % TimeSpan.TicksPerSecond));
    }
  }
}
