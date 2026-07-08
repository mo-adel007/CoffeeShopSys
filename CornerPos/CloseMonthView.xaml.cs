using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Admin "Close Month" screen (ports Control Panel "Close Month" tab). Shows the
    /// running month totals in four cards (income, purchases, monthly expenses, profit)
    /// with the per-day rows and the monthly expense rows in two grids. Closing the
    /// month snapshots the totals into month_details, then clears day_details and
    /// monthly_expenses so the next month starts fresh.
    /// </summary>
    public partial class CloseMonthView : UserControl
    {
        private decimal _totalIncome;
        private decimal _totalBuy;
        private decimal _totalMonthlyExpenses;
        private decimal _profit;

        public CloseMonthView()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            // Days: income + purchases.
            var days = Data.Query("SELECT * FROM day_details;");
            DaysGrid.ItemsSource = days.DefaultView;
            _totalIncome = Sum(days, "TotalSell");
            _totalBuy = Sum(days, "TotalBuy");

            // Monthly expenses.
            var expenses = Data.Query("SELECT expens_name, Price FROM monthly_expenses;");
            ExpensesGrid.ItemsSource = expenses.DefaultView;
            _totalMonthlyExpenses = Sum(expenses, "Price");

            _profit = _totalIncome - _totalBuy - _totalMonthlyExpenses;

            IncomeText.Text = _totalIncome.ToString("0.00");
            PurchasesText.Text = _totalBuy.ToString("0.00");
            ExpensesText.Text = _totalMonthlyExpenses.ToString("0.00");
            ProfitText.Text = _profit.ToString("0.00");

            CloseMonthBtn.IsEnabled = true;
        }

        private void CloseMonth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int monthNum = Convert.ToInt32(Data.Scalar("SELECT COUNT(*) FROM month_details;")) + 1;

                Data.Execute(
                    "INSERT INTO month_details (MonthNumber, TotalSell, TotalBuy, ProfitDay, TotalMonthlyExpenses) " +
                    "VALUES (@m,@ts,@tb,@p,@me);",
                    ("@m", monthNum), ("@ts", _totalIncome), ("@tb", _totalBuy),
                    ("@p", _profit), ("@me", _totalMonthlyExpenses));

                Data.Execute("DELETE FROM day_details;");
                Data.Execute("DELETE FROM monthly_expenses;");

                CloseMonthBtn.IsEnabled = false;
                MessageBox.Show("تم تقفيل الشهر", "Corner", MessageBoxButton.OK, MessageBoxImage.Information);
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Corner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Sum a numeric column, treating NULL / missing values as 0.</summary>
        private static decimal Sum(DataTable table, string column)
        {
            decimal total = 0m;
            foreach (DataRow row in table.Rows)
                total += row[column] == DBNull.Value ? 0m : Convert.ToDecimal(row[column]);
            return total;
        }
    }
}
