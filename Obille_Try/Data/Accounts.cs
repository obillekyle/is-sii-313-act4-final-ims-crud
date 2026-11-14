using System.Data.OleDb;

namespace Obille_Try.Data {
  public sealed record Account(int Id, string Username, string FullName, string Role);

  public enum SignInStatus { SignedIn, WrongCredentials, Inactive }

  // Sign in and registration against the Users table. Every value goes in
  // as a parameter: OLE DB's parameters are positional, one ? each, added
  // in the order they appear, and the provider passes them apart from the
  // SQL text, so nothing typed into a form can change the query.
  public static class Accounts {
    public const int MinPasswordLength = 8;

    public static Account? Current { get; set; }

    // Hashed once, for a username nobody has: checking a password against
    // it takes as long as against a real account's hash.
    static readonly Lazy<string> standIn = new(() => PasswordHelper.Hash(Guid.NewGuid().ToString()));

    // A wrong username and a wrong password get the same answer in the same
    // time, so the form tells nobody which usernames exist.
    public static (SignInStatus Status, Account? Account) SignIn(string username, string password) {
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand(
          "SELECT UserID, Username, PasswordHash, FullName, UserRole, IsActive FROM Users WHERE Username = ?", con);
      cmd.Parameters.AddWithValue("@Username", username.Trim());
      using var reader = cmd.ExecuteReader();
      if (!reader.Read()) {
        PasswordHelper.Verify(password, standIn.Value);
        return (SignInStatus.WrongCredentials, null);
      }

      var storedHash = Convert.ToString(reader["PasswordHash"]) ?? "";
      if (!PasswordHelper.Verify(password, storedHash)) return (SignInStatus.WrongCredentials, null);
      if (!Convert.ToBoolean(reader["IsActive"])) return (SignInStatus.Inactive, null);

      var account = new Account(
          Convert.ToInt32(reader["UserID"]),
          Convert.ToString(reader["Username"]) ?? "",
          Convert.ToString(reader["FullName"]) ?? "",
          Convert.ToString(reader["UserRole"]) ?? "");
      return (SignInStatus.SignedIn, account);
    }

    // Access compares text without case, so "Juan" and "juan" are one name
    // here, as the unique index on Username has them.
    public static bool UsernameTaken(string username) {
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand("SELECT COUNT(*) FROM Users WHERE Username = ?", con);
      cmd.Parameters.AddWithValue("@Username", username.Trim());
      return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    // Saves a new active account with the User role. The form has checked
    // the fields; the name is checked again here, since another window
    // could have taken it since.
    public static bool Register(string fullName, string username, string password) {
      if (password.Length < MinPasswordLength)
        throw new ArgumentException($"A password needs at least {MinPasswordLength} characters.", nameof(password));
      if (UsernameTaken(username)) return false;

      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand(
          "INSERT INTO Users (Username, PasswordHash, FullName, UserRole, IsActive, CreatedAt) VALUES (?, ?, ?, ?, ?, ?)", con);
      cmd.Parameters.AddWithValue("@Username", username.Trim());
      cmd.Parameters.AddWithValue("@PasswordHash", PasswordHelper.Hash(password));
      cmd.Parameters.AddWithValue("@FullName", fullName.Trim());
      cmd.Parameters.AddWithValue("@UserRole", "User");
      cmd.Parameters.AddWithValue("@IsActive", true);
      // A DateTime parameter goes in as DBTimeStamp, which Access refuses
      // with fractional seconds; Date is Access's own date type.
      cmd.Parameters.Add("@CreatedAt", OleDbType.Date).Value = DateTime.Now;
      return cmd.ExecuteNonQuery() == 1;
    }

    // User management: the Users page lists, adds, edits and deletes
    // accounts. Two rules hold whoever is signed in: nobody deletes or
    // deactivates their own account from here, and at least one active
    // Admin is always left, so the table can always be managed.
    public static readonly string[] Roles = { "Admin", "User" };

    public static List<UserRow> List() {
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand(
          "SELECT UserID, Username, FullName, UserRole, IsActive, CreatedAt FROM Users ORDER BY FullName", con);
      using var r = cmd.ExecuteReader();
      var list = new List<UserRow>();
      while (r.Read()) {
        list.Add(new UserRow(
            Convert.ToInt32(r["UserID"]),
            Convert.ToString(r["Username"]) ?? "",
            Convert.ToString(r["FullName"]) ?? "",
            Convert.ToString(r["UserRole"]) ?? "",
            Convert.ToBoolean(r["IsActive"]),
            Convert.ToDateTime(r["CreatedAt"])));
      }
      return list;
    }

    public static bool UsernameTaken(string username, int exceptId) {
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand("SELECT COUNT(*) FROM Users WHERE Username = ? AND UserID <> ?", con);
      cmd.Parameters.AddWithValue("@Username", username.Trim());
      cmd.Parameters.AddWithValue("@UserID", exceptId);
      return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    // Why a change would take away the table's last active Admin, or null
    // when it would not: only an account that is an active Admin now, and
    // would not be after, can break the rule. A delete is checked as a
    // change to an inactive User.
    public static string? AdminRuleBroken(int userId, string newRole, bool newActive) {
      if (newRole == "Admin" && newActive) return null;
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand(
          "SELECT SUM(IIF(UserID = ?, 1, 0)), SUM(IIF(UserID <> ?, 1, 0)) FROM Users WHERE UserRole = 'Admin' AND IsActive = True", con);
      cmd.Parameters.AddWithValue("@UserID", userId);
      cmd.Parameters.AddWithValue("@OtherID", userId);
      using var r = cmd.ExecuteReader();
      r.Read();
      var isAdminNow = !r.IsDBNull(0) && Convert.ToInt32(r[0]) > 0;
      var others = r.IsDBNull(1) ? 0 : Convert.ToInt32(r[1]);
      return isAdminNow && others == 0 ? "At least one active Admin has to stay." : null;
    }

    public static UserRow Create(string fullName, string username, string password, string role, bool active) {
      if (password.Length < MinPasswordLength)
        throw new ArgumentException($"A password needs at least {MinPasswordLength} characters.", nameof(password));
      using var con = Database.GetConnection();
      con.Open();
      var created = Database.Now();
      using (var cmd = new OleDbCommand(
          "INSERT INTO Users (Username, PasswordHash, FullName, UserRole, IsActive, CreatedAt) VALUES (?, ?, ?, ?, ?, ?)", con)) {
        cmd.Parameters.AddWithValue("@Username", username.Trim());
        cmd.Parameters.AddWithValue("@PasswordHash", PasswordHelper.Hash(password));
        cmd.Parameters.AddWithValue("@FullName", fullName.Trim());
        cmd.Parameters.AddWithValue("@UserRole", role);
        cmd.Parameters.AddWithValue("@IsActive", active);
        cmd.Parameters.Add("@CreatedAt", OleDbType.Date).Value = created;
        cmd.ExecuteNonQuery();
      }
      using var id = new OleDbCommand("SELECT @@IDENTITY", con);
      return new UserRow(Convert.ToInt32(id.ExecuteScalar()), username.Trim(), fullName.Trim(), role, active, created);
    }

    // A null or empty password keeps the one the account has.
    public static void Update(UserRow user, string? newPassword) {
      using var con = Database.GetConnection();
      con.Open();
      var setPassword = !string.IsNullOrEmpty(newPassword);
      using var cmd = new OleDbCommand(
          "UPDATE Users SET Username = ?, FullName = ?, UserRole = ?, IsActive = ?" +
          (setPassword ? ", PasswordHash = ?" : "") + " WHERE UserID = ?", con);
      cmd.Parameters.AddWithValue("@Username", user.Username.Trim());
      cmd.Parameters.AddWithValue("@FullName", user.FullName.Trim());
      cmd.Parameters.AddWithValue("@UserRole", user.Role);
      cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
      if (setPassword) cmd.Parameters.AddWithValue("@PasswordHash", PasswordHelper.Hash(newPassword!));
      cmd.Parameters.AddWithValue("@UserID", user.Id);
      if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException($"{user.FullName} is no longer in the table.");

      // The sidebar shows the signed-in person's name and role.
      if (Current is { } me && me.Id == user.Id) Current = new Account(user.Id, user.Username, user.FullName, user.Role);
    }

    public static void Delete(int userId) {
      using var con = Database.GetConnection();
      con.Open();
      using var cmd = new OleDbCommand("DELETE FROM Users WHERE UserID = ?", con);
      cmd.Parameters.AddWithValue("@UserID", userId);
      cmd.ExecuteNonQuery();
    }
  }

  public sealed record UserRow(int Id, string Username, string FullName, string Role, bool IsActive, DateTime CreatedAt);
}
