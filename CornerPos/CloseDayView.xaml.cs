using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Cashier "Close Day" screen (ports the worker app's Close Day tab). Shows the day's
    /// sales and purchases (close_day rows split by processT), with Income / Expenses / Profit
    /// totals. "Close day" snapshots the totals into day_details as the next DayNum, then clears
    /// close_day, shift_details and user_shift (in that order) to start a fresh day.
    /// </summary>
    public partial class CloseDayView : UserControl
    {
        private decimal totalSell;
        private decimal totalBuy;
        private decimal profit;

        public CloseDayView()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            // Sales
            var sells = Data.Query("SELECT proName, price, quantity, Dtime FROM close_day WHERE processT='sell';");
            SellGrid.ItemsSource = sells.DefaultView;
            totalSell = SumPrice(sells);

            // Purchases
            var buys = Data.Query("SELECT proName, price, quantity, Dtime FROM close_day WHERE processT='buy';");
            BuyGrid.ItemsSource = buys.DefaultView;
            totalBuy = SumPrice(buys);

            profit = totalSell - totalBuy;

            IncomeText.Text = totalSell.ToString("0.00");
            ExpensesText.Text = totalBuy.ToString("0.00");
            ProfitText.Text = profit.ToString("0.00");

            CloseDayBtn.IsEnabled = true;
        }

        private static decimal SumPrice(DataTable dt)
        {
            decimal sum = 0m;
            foreach (DataRow r in dt.Rows)
                sum += r["price"] == DBNull.Value ? 0m : Convert.ToDecimal(r["price"]);
            return sum;
        }

        private void CloseDay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int dayNum = Convert.ToInt32(Data.Scalar("SELECT COUNT(*) FROM day_details;")) + 1;
                Data.Execute(
                    "INSERT INTO day_details (DayNum, TotalSell, TotalBuy, ProfitDay) VALUES (@d,@ts,@tb,@p);",
                    ("@d", dayNum), ("@ts", totalSell), ("@tb", totalBuy), ("@p", profit));

                // Clear in order (preserve the cascade).
                Data.Execute("DELETE FROM close_day;");
                Data.Execute("DELETE FROM shift_details;");
                Data.Execute("DELETE FROM user_shift;");

                CloseDayBtn.IsEnabled = false;
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
