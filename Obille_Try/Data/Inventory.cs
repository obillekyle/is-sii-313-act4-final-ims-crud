using System.Data.OleDb;

namespace Obille_Try.Data {
  // The materials, in the Materials table of the same Access file as the
  // accounts. Every value goes in as a ? parameter, in the order the SQL
  // names them. The table is made, and filled with the 59 sample materials,
  // the first time the app opens a file that lacks it, so an older copy of
  // the database keeps working.
  public static class Inventory {
    // Raised after every add, edit, delete and undo, so the dashboard and
    // the low stock menu read the table again.
    public static event EventHandler? Changed;

    const string Columns =
        "Code, MaterialName, Category, Unit, UnitCost, Stock, ReorderPoint, ReorderQuantity, Supplier, Bin";

    public static void EnsureTable() {
      using var con = Database.GetConnection();
      con.Open();
      var tables = con.GetSchema("Tables", new string?[] { null, null, "Materials", "TABLE" });
      if (tables.Rows.Count > 0) return;

      foreach (var sql in new[] {
          "CREATE TABLE Materials (MaterialID COUNTER CONSTRAINT PK_Materials PRIMARY KEY, Code TEXT(20) NOT NULL, " +
          "MaterialName TEXT(150) NOT NULL, Category TEXT(60) NOT NULL, Unit TEXT(20) NOT NULL, UnitCost CURRENCY NOT NULL, " +
          "Stock LONG NOT NULL, ReorderPoint LONG NOT NULL, ReorderQuantity LONG NOT NULL, Supplier TEXT(100), Bin TEXT(60), " +
          "CreatedAt DATETIME NOT NULL, UpdatedAt DATETIME NOT NULL)",
          "CREATE UNIQUE INDEX UX_Materials_Code ON Materials (Code)" }) {
        using var ddl = new OleDbCommand(sql, con);
        ddl.ExecuteNonQuery();
      }
      // Reorder quantity is not in the sketch's rows: twice the reorder
      // point, the figure the edit dialog showed before it was saved.
      foreach (var m in SampleData.Materials) Insert(con, m with { ReorderQuantity = m.ReorderPoint * 2 }, null);
    }

    public static List<MaterialRow> All() {
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand($"SELECT MaterialID, {Columns} FROM Materials ORDER BY MaterialName", con);
      using var r = cmd.ExecuteReader();
      var list = new List<MaterialRow>();
      while (r.Read()) {
        list.Add(new MaterialRow(
            Name: Convert.ToString(r["MaterialName"]) ?? "",
            Code: Convert.ToString(r["Code"]) ?? "",
            Bin: r["Bin"] as string ?? "",
            Stock: Convert.ToInt32(r["Stock"]),
            Unit: Convert.ToString(r["Unit"]) ?? "",
            Cost: Convert.ToDecimal(r["UnitCost"]),
            ReorderPoint: Convert.ToInt32(r["ReorderPoint"]),
            Category: Convert.ToString(r["Category"]) ?? "",
            Supplier: r["Supplier"] as string ?? "",
            ReorderQuantity: Convert.ToInt32(r["ReorderQuantity"]),
            Id: Convert.ToInt32(r["MaterialID"])));
      }
      return list;
    }

    // Access compares text without case, so "stl-0302" and "STL-0302" are
    // one code, as the unique index has them.
    public static bool CodeTaken(string code, int exceptId = 0) {
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand("SELECT COUNT(*) FROM Materials WHERE Code = ? AND MaterialID <> ?", con);
      cmd.Parameters.AddWithValue("@Code", code.Trim());
      cmd.Parameters.AddWithValue("@MaterialID", exceptId);
      return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    // Saves a new material and returns it with the ID Access gave it.
    public static MaterialRow Add(MaterialRow m) {
      using var con = Database.GetConnection();
      con.Open();
      Insert(con, m, null);
      using var id = new OleDbCommand("SELECT @@IDENTITY", con);
      var saved = m with { Id = Convert.ToInt32(id.ExecuteScalar()) };
      Changed?.Invoke(null, EventArgs.Empty);
      return saved;
    }

    public static void Update(MaterialRow m) {
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand(
          "UPDATE Materials SET Code = ?, MaterialName = ?, Category = ?, Unit = ?, UnitCost = ?, Stock = ?, " +
          "ReorderPoint = ?, ReorderQuantity = ?, Supplier = ?, Bin = ?, UpdatedAt = ? WHERE MaterialID = ?", con);
      AddValues(cmd, m);
      cmd.Parameters.Add("@UpdatedAt", OleDbType.Date).Value = Database.Now();
      cmd.Parameters.Add("@MaterialID", OleDbType.Integer).Value = m.Id;
      if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException($"{m.Name} is no longer in the table.");
      Changed?.Invoke(null, EventArgs.Empty);
    }

    public static void Delete(IReadOnlyList<MaterialRow> rows) {
      using var con = Database.GetConnection();
      con.Open();
      using var tx = con.BeginTransaction();
      foreach (var m in rows) {
        using var cmd = new OleDbCommand("DELETE FROM Materials WHERE MaterialID = ?", con, tx);
        cmd.Parameters.Add("@MaterialID", OleDbType.Integer).Value = m.Id;
        cmd.ExecuteNonQuery();
      }
      tx.Commit();
      Changed?.Invoke(null, EventArgs.Empty);
    }

    // Undo of a delete: the rows go back under their own IDs, which Access
    // accepts for an AutoNumber column when the value is given.
    public static void Restore(IReadOnlyList<MaterialRow> rows) {
      using var con = Database.GetConnection();
      con.Open();
      using var tx = con.BeginTransaction();
      foreach (var m in rows) Insert(con, m, tx, keepId: true);
      tx.Commit();
      Changed?.Invoke(null, EventArgs.Empty);
    }

    static void Insert(OleDbConnection con, MaterialRow m, OleDbTransaction? tx, bool keepId = false) {
      var idColumn = keepId ? "MaterialID, " : "";
      var idMark = keepId ? "?, " : "";
      using var cmd = new OleDbCommand(
          $"INSERT INTO Materials ({idColumn}{Columns}, CreatedAt, UpdatedAt) VALUES ({idMark}?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
          con, tx);
      if (keepId) cmd.Parameters.Add("@MaterialID", OleDbType.Integer).Value = m.Id;
      AddValues(cmd, m);
      var now = Database.Now();
      cmd.Parameters.Add("@CreatedAt", OleDbType.Date).Value = now;
      cmd.Parameters.Add("@UpdatedAt", OleDbType.Date).Value = now;
      cmd.ExecuteNonQuery();
    }

    // In the order of Columns.
    static void AddValues(OleDbCommand cmd, MaterialRow m) {
      cmd.Parameters.Add("@Code", OleDbType.VarWChar).Value = m.Code.Trim();
      cmd.Parameters.Add("@MaterialName", OleDbType.VarWChar).Value = m.Name.Trim();
      cmd.Parameters.Add("@Category", OleDbType.VarWChar).Value = m.Category;
      cmd.Parameters.Add("@Unit", OleDbType.VarWChar).Value = m.Unit;
      cmd.Parameters.Add("@UnitCost", OleDbType.Currency).Value = m.Cost;
      cmd.Parameters.Add("@Stock", OleDbType.Integer).Value = m.Stock;
      cmd.Parameters.Add("@ReorderPoint", OleDbType.Integer).Value = m.ReorderPoint;
      cmd.Parameters.Add("@ReorderQuantity", OleDbType.Integer).Value = m.ReorderQuantity;
      cmd.Parameters.Add("@Supplier", OleDbType.VarWChar).Value = m.Supplier.Length > 0 ? m.Supplier : DBNull.Value;
      cmd.Parameters.Add("@Bin", OleDbType.VarWChar).Value = m.Bin.Trim().Length > 0 ? m.Bin.Trim() : DBNull.Value;
    }
  }
}
