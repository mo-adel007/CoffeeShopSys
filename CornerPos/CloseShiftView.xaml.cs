using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Close Shift screen (ports the worker "Close Shift" tab). Shows the current
    /// shift's sales and purchases from the close_shift table in two grids, with
    /// Income / Expenses / Net totals on top. Closing the shift snapshots the
    /// totals into shift_details and clears close_shift, then resets to zero.
    /// </summary>
    public partial class CloseShiftView : UserControl
    {
        private readonly int _shift;
        private readonly int _userId;
        private decimal _totalSell;
        private decimal _totalBuy;

        public CloseShiftView(int shift, int userId)
        {
            InitializeComponent();
            _shift = shift;
            _userId = userId;
            Refresh();
        }

        private void Refresh()
        {
            // Sales — only this cashier's own rows for the current shift
            var sells = Data.Query(
                "SELECT proName, price, quantity, Dtime FROM close_shift WHERE processT='sell' AND ShiftNumber=@s AND Userid=@u;",
                ("@s", _shift), ("@u", _userId));
            SellsGrid.ItemsSource = sells.DefaultView;
            _totalSell = SumPrice(sells);

            // Purchases
            var buys = Data.Query(
                "SELECT proName, price, quantity, Dtime FROM close_shift WHERE processT='buy' AND ShiftNumber=@s AND Userid=@u;",
                ("@s", _shift), ("@u", _userId));
            BuysGrid.ItemsSource = buys.DefaultView;
            _totalBuy = SumPrice(buys);

            decimal net = _totalSell - _totalBuy;

            IncomeText.Text = _totalSell.ToString("0.00");
            ExpensesText.Text = _totalBuy.ToString("0.00");
            NetText.Text = net.ToString("0.00");
        }

        private static decimal SumPrice(DataTable dt)
        {
            decimal total = 0m;
            foreach (DataRow r in dt.Rows)
                total += r["price"] == DBNull.Value ? 0m : Convert.ToDecimal(r["price"]);
            return total;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            decimal net = _totalSell - _totalBuy;
            try
            {
                Data.Execute(
                    "INSERT INTO shift_details (ShiftNum, TotalSell, TotalBuy, ProfitShift) VALUES (@sh,@ts,@tb,@p);",
                    ("@sh", _shift), ("@ts", _totalSell), ("@tb", _totalBuy), ("@p", net));
                Data.Execute("DELETE FROM close_shift WHERE Userid=@u AND ShiftNumber=@s;",
                    ("@u", _userId), ("@s", _shift));

                CloseBtn.IsEnabled = false;
                MessageBox.Show("تم تقفيل الوردية", "Corner", MessageBoxButton.OK, MessageBoxImage.Information);
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Corner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
