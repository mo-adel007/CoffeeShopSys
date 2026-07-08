using System;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Admin "Close Month" screen. Rolls up the daily closes (day_details) for the
    /// month, keeping product sales isolated from cash movements: product sales,
    /// safe deposits and expenses/withdrawals are each shown separately, plus the
    /// recurring monthly expenses. Closing snapshots month_details and clears
    /// day_details + monthly_expenses.
    /// </summary>
    public partial class CloseMonthView : UserControl
    {
        private decimal _sales, _deposits, _out, _monthly, _profit;

        public CloseMonthView()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            DaysGrid.ItemsSource = Data.Query(
                "SELECT DayNum AS \"اليوم\", TotalSell AS \"المبيعات\", TotalDeposits AS \"الإيداعات\", " +
                "TotalBuy AS \"مصروفات ومسحوبات\", ProfitDay AS \"الربح\" FROM day_details;").DefaultView;
            ExpensesGrid.ItemsSource = Data.Query(
                "SELECT expens_name AS \"المصروف\", Price AS \"المبلغ\" FROM monthly_expenses;").DefaultView;

            _sales = Scalar("SELECT COALESCE(SUM(TotalSell),0) FROM day_details;");
            _deposits = Scalar("SELECT COALESCE(SUM(TotalDeposits),0) FROM day_details;");
            _out = Scalar("SELECT COALESCE(SUM(TotalBuy),0) FROM day_details;");
            _monthly = Scalar("SELECT COALESCE(SUM(Price),0) FROM monthly_expenses;");
            _profit = _sales - _out - _monthly;

            IncomeText.Text = _sales.ToString("0.00");
            DepositsText.Text = _deposits.ToString("0.00");
            ExpensesText.Text = (_out + _monthly).ToString("0.00");
            ProfitText.Text = _profit.ToString("0.00");

            CloseMonthBtn.IsEnabled = true;
        }

        private void CloseMonth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int monthNum = Convert.ToInt32(Data.Scalar("SELECT COUNT(*) FROM month_details;")) + 1;

                Data.Execute(
                    "INSERT INTO month_details (MonthNumber, TotalSell, TotalBuy, TotalDeposits, ProfitDay, TotalMonthlyExpenses) " +
                    "VALUES (@m,@ts,@tb,@dep,@p,@me);",
                    ("@m", monthNum), ("@ts", _sales), ("@tb", _out), ("@dep", _deposits),
                    ("@p", _profit), ("@me", _monthly));

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

        private static decimal Scalar(string sql)
        {
            object o = Data.Scalar(sql);
            return o == null || o == DBNull.Value ? 0m : Convert.ToDecimal(o);
        }
    }
}
