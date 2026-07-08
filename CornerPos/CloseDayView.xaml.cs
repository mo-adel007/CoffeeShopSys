using System;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Close Day screen (whole day, all cashiers). Product sales (processT='sale')
    /// and cash-safe movements (deposit / withdrawal / purchase / expense) are kept
    /// as SEPARATE ledgers. Closing snapshots the day into day_details and clears the
    /// day cascade (close_day, shift_details, user_shift).
    /// </summary>
    public partial class CloseDayView : UserControl
    {
        private const string MovementFilter = "processT IN ('deposit','withdrawal','purchase','expense')";
        private const string OutFilter = "processT IN ('withdrawal','purchase','expense')";
        private const string MovementTypeCase =
            "CASE processT WHEN 'deposit' THEN 'إيداع' WHEN 'withdrawal' THEN 'سحب' " +
            "WHEN 'purchase' THEN 'شراء' WHEN 'expense' THEN 'مصروف' ELSE processT END";

        private decimal _sales, _deposits, _out;

        public CloseDayView()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            SellsGrid.ItemsSource = Data.Query(
                "SELECT proName AS \"الصنف\", quantity AS \"الكمية\", price AS \"المبلغ\", Dtime AS \"الوقت\" " +
                "FROM close_day WHERE processT='sale';").DefaultView;
            MovesGrid.ItemsSource = Data.Query(
                "SELECT " + MovementTypeCase + " AS \"النوع\", proName AS \"البيان\", price AS \"المبلغ\", Dtime AS \"الوقت\" " +
                "FROM close_day WHERE " + MovementFilter + ";").DefaultView;

            _sales = Sum("processT='sale'");
            _deposits = Sum("processT='deposit'");
            _out = Sum(OutFilter);

            SalesText.Text = _sales.ToString("0.00");
            DepositsText.Text = _deposits.ToString("0.00");
            OutText.Text = _out.ToString("0.00");
        }

        private decimal Sum(string where)
        {
            object o = Data.Scalar("SELECT COALESCE(SUM(price),0) FROM close_day WHERE " + where + ";");
            return o == null || o == DBNull.Value ? 0m : Convert.ToDecimal(o);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int dayNum = Convert.ToInt32(Data.Scalar("SELECT COUNT(*) FROM day_details;")) + 1;
                Data.Execute(
                    "INSERT INTO day_details (DayNum, TotalSell, TotalBuy, TotalDeposits, ProfitDay) " +
                    "VALUES (@d,@ts,@tb,@dep,@p);",
                    ("@d", dayNum), ("@ts", _sales), ("@tb", _out), ("@dep", _deposits), ("@p", _sales - _out));

                // Day cascade: clear the day's aggregation + the shift tables.
                Data.Execute("DELETE FROM close_day;");
                Data.Execute("DELETE FROM shift_details;");
                Data.Execute("DELETE FROM user_shift;");

                CloseBtn.IsEnabled = false;
                MessageBox.Show("تم تقفيل اليوم", "Corner", MessageBoxButton.OK, MessageBoxImage.Information);
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Corner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
