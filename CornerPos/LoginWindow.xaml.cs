using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Input;
using Corner_Application; // shared Db + Security

namespace CornerPos
{
    /// <summary>
    /// Modern login window. Looks the user up by username (parameterized — no SQL
    /// injection) and verifies the typed password against the stored PBKDF2 hash.
    /// A legacy plaintext row is transparently re-hashed on first successful login.
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => Username.Focus();
        }

        private void Header_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Login_Click(object sender, RoutedEventArgs e) => DoLogin();

        private void Pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) DoLogin();
        }

        private void DoLogin()
        {
            Error.Text = "";
            string user = (Username.Text ?? "").Trim();
            string pass = Password.Password;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                Error.Text = "Please enter your username and password.";
                return;
            }

            try
            {
                DataRow row = null;
                using (var conn = new SQLiteConnection(Db.ConnectionString()))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT id, username, Pass, time_work FROM login WHERE username = @u;", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", user);
                        var dt = new DataTable();
                        using (var da = new SQLiteDataAdapter(cmd)) da.Fill(dt);
                        if (dt.Rows.Count == 1) row = dt.Rows[0];
                    }
                }

                string stored = row == null ? null : Convert.ToString(row["Pass"]);
                if (row == null || !Security.Verify(pass, stored))
                {
                    Error.Text = "Incorrect username or password.";
                    return;
                }

                int userId = Convert.ToInt32(row["id"]);
                if (Security.IsLegacyPlaintext(stored))
                    UpgradeHash(userId.ToString(), pass);

                string name = Convert.ToString(row["username"]);
                string role = Convert.ToString(row["time_work"]);

                // Cashiers get a real shift number (reused or opened) per the legacy
                // user_shift logic; admins use 0.
                int shift = string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase)
                    ? 0 : Shift.AssignForCashier(name);

                var main = new MainWindow(userId, name, role, shift, DateTime.Now);
                main.Show();
                Close();
            }
            catch (Exception ex)
            {
                Error.Text = ex.Message;
            }
        }

        private void UpgradeHash(string id, string plaintext)
        {
            try
            {
                using (var conn = new SQLiteConnection(Db.ConnectionString()))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("UPDATE login SET Pass = @p WHERE id = @id;", conn))
                    {
                        cmd.Parameters.AddWithValue("@p", Security.Hash(plaintext));
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { /* login still succeeds even if the re-hash can't be persisted */ }
        }
    }
}
