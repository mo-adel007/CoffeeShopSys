using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Corner_Application; // Security.Hash

namespace CornerPos
{
    /// <summary>
    /// Admin Users screen (ports Control Panel "Users" tab). A grid of logins on the
    /// left; selecting a row loads it into the add/edit form on the right. Add / Update
    /// / Delete against the login table, parameterized.
    ///
    /// SECURITY: passwords are stored one-way hashed (see <see cref="Security"/>) and are
    /// NEVER selected from the database nor displayed. The Pass column is never queried.
    /// On edit the password box is left blank; a non-empty value RESETS the password, a
    /// blank value leaves the existing hash untouched.
    /// </summary>
    public partial class UsersView : UserControl
    {
        private int _selectedId; // 0 = adding a new user

        public UsersView()
        {
            InitializeComponent();
            LoadGrid();
            ClearForm();
        }

        private void LoadGrid()
        {
            // Note: Pass is deliberately NOT selected — the hash never leaves the DB layer.
            Grid.ItemsSource = Data.Query(
                "SELECT id, username, time_work FROM login ORDER BY username;").DefaultView;
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = Grid.SelectedItem as DataRowView;
            if (row == null) return;

            _selectedId = Convert.ToInt32(row["id"]);
            NameBox.Text = Convert.ToString(row["username"]);

            // Map stored role -> combo. "admin" => Administrator, anything else => Cashier.
            string role = Convert.ToString(row["time_work"]);
            RoleBox.SelectedValue = string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase)
                ? "admin" : "user";

            // Never populate the password box.
            PassBox.Password = "";
            PassHint.Visibility = Visibility.Visible;

            FormTitle.Text = "Edit user";
            SaveBtn.Content = "Save changes";
            DeleteBtn.IsEnabled = true;
            Error.Text = "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Error.Text = "";
            string name = (NameBox.Text ?? "").Trim();
            if (name.Length == 0) { Error.Text = "Username is required."; return; }

            // Role required; default to "user" if nothing is picked.
            string role = RoleBox.SelectedValue as string;
            if (string.IsNullOrEmpty(role)) role = "user";

            string password = PassBox.Password ?? "";

            try
            {
                if (_selectedId == 0)
                {
                    // New user: password is required.
                    if (password.Length == 0) { Error.Text = "Password is required."; return; }

                    long dup = Convert.ToInt64(Data.Scalar(
                        "SELECT COUNT(*) FROM login WHERE username=@u;", ("@u", name)));
                    if (dup > 0) { Error.Text = "That username already exists."; return; }

                    Data.Execute(
                        "INSERT INTO login (username, Pass, time_work) VALUES (@u, @p, @t);",
                        ("@u", name), ("@p", Security.Hash(password)), ("@t", role));
                }
                else
                {
                    // Edit: only touch Pass when a new password was typed.
                    if (password.Length == 0)
                    {
                        Data.Execute(
                            "UPDATE login SET username=@u, time_work=@t WHERE id=@id;",
                            ("@u", name), ("@t", role), ("@id", _selectedId));
                    }
                    else
                    {
                        Data.Execute(
                            "UPDATE login SET username=@u, time_work=@t, Pass=@p WHERE id=@id;",
                            ("@u", name), ("@t", role), ("@p", Security.Hash(password)), ("@id", _selectedId));
                    }
                }

                LoadGrid();
                ClearForm();
            }
            catch (Exception ex) { Error.Text = ex.Message; }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("Delete this user?", "Corner",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                Data.Execute("DELETE FROM login WHERE id=@id;", ("@id", _selectedId));
                LoadGrid();
                ClearForm();
            }
            catch (Exception ex) { Error.Text = ex.Message; }
        }

        private void New_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _selectedId = 0;
            Grid.SelectedItem = null;
            NameBox.Text = "";
            PassBox.Password = "";
            PassHint.Visibility = Visibility.Collapsed;
            RoleBox.SelectedIndex = -1;
            FormTitle.Text = "Add user";
            SaveBtn.Content = "Add user";
            DeleteBtn.IsEnabled = false;
            Error.Text = "";
        }
    }
}
