using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Admin Monthly Expenses screen (ports Control Panel "Monthly Expenses" tab).
    /// The grid on the left lists the current month's recurring expenses from
    /// <c>monthly_expenses</c>. The form on the right is an entry form: adding writes
    /// the expense to BOTH <c>monthly_expenses</c> (the live list) and
    /// <c>all_monthlyexpenses</c> (the permanent, append-only history log) inside a
    /// single transaction. Deleting removes only from <c>monthly_expenses</c> so the
    /// historical log is never touched. No edit/update - Add + Delete only.
    /// </summary>
    public partial class MonthlyExpensesView : UserControl
    {
        private int _selectedId; // 0 = nothing selected

        public MonthlyExpensesView()
        {
            InitializeComponent();
            LoadGrid();
            ClearForm();
        }

        private void LoadGrid()
        {
            Grid.ItemsSource = Data.Query(
                "SELECT id_expenses, expens_name, Price FROM monthly_expenses ORDER BY expens_name;").DefaultView;
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = Grid.SelectedItem as DataRowView;
            if (row == null) return;

            _selectedId = Convert.ToInt32(row["id_expenses"]);
            DeleteBtn.IsEnabled = true;
            Error.Text = "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Error.Text = "";
            string name = (NameBox.Text ?? "").Trim();
            if (name.Length == 0) { Error.Text = "الاسم مطلوب."; return; }

            decimal price;
            if (!decimal.TryParse((PriceBox.Text ?? "").Trim(), out price))
            { Error.Text = "السعر يجب أن يكون رقماً."; return; }

            try
            {
                string when = DateTime.Now.ToString("s");

                // Add to the live list AND the permanent history log atomically.
                Data.Batch(new List<(string, (string, object)[])>
                {
                    (
                        "INSERT INTO monthly_expenses (expens_name, Price) VALUES (@n, @p);",
                        new (string, object)[] { ("@n", name), ("@p", price) }
                    ),
                    (
                        "INSERT INTO all_monthlyexpenses (expens_name, Price, Date_time) VALUES (@n, @p, @d);",
                        new (string, object)[] { ("@n", name), ("@p", price), ("@d", when) }
                    )
                });

                LoadGrid();
                ClearForm();
            }
            catch (Exception ex) { Error.Text = ex.Message; }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("حذف هذا المصروف؟", "Corner",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                // Remove only from the live list; the history log is permanent.
                Data.Execute("DELETE FROM monthly_expenses WHERE id_expenses=@id;", ("@id", _selectedId));
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
            PriceBox.Text = "";
            DeleteBtn.IsEnabled = false;
            Error.Text = "";
        }
    }
}
