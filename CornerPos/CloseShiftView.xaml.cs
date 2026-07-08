using System;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Close Shift screen. Product sales (processT='sale') and cash-safe movements
    /// (deposit / withdrawal / purchase / expense) are kept as SEPARATE ledgers — a
    /// deposit is never counted as a sale. Shown per current cashier + shift. Closing
    /// snapshots the shift into shift_details and clears this cashier's close_shift rows.
    /// </summary>
    public partial class CloseShiftView : UserControl
    {
        private const string MovementFilter = "processT IN ('deposit','withdrawal','purchase','expense')";
        private const string OutFilter = "processT IN ('withdrawal','purchase','expense')";
        private const string MovementTypeCase =
            "CASE processT WHEN 'deposit' THEN 'إيداع' WHEN 'withdrawal' THEN 'سحب' " +
            "WHEN 'purchase' THEN 'شراء' WHEN 'expense' THEN 'مصروف' ELSE processT END";

        private readonly int _shift;
        private readonly int _userId;
        private decimal _sales, _deposits, _out;

        public CloseShiftView(int shift, int userId)
        {
            InitializeComponent();
            _shift = shift;
            _userId = userId;
            Refresh();
        }

        private void Refresh()
        {
            string scope = " AND ShiftNumber=@s AND Userid=@u";

            // Product sales ledger
            SellsGrid.ItemsSource = Data.Query(
                "SELECT proName AS \"الصنف\", quantity AS \"الكمية\", price AS \"المبلغ\", Dtime AS \"الوقت\" " +
                "FROM close_shift WHERE processT='sale'" + scope + ";",
                ("@s", _shift), ("@u", _userId)).DefaultView;

            // Cash / safe movements ledger (isolated from sales)
            MovesGrid.ItemsSource = Data.Query(
                "SELECT " + MovementTypeCase + " AS \"النوع\", proName AS \"البيان\", price AS \"المبلغ\", Dtime AS \"الوقت\" " +
                "FROM close_shift WHERE " + MovementFilter + scope + ";",
                ("@s", _shift), ("@u", _userId)).DefaultView;

            _sales = Sum("processT='sale'");
            _deposits = Sum("processT='deposit'");
            _out = Sum(OutFilter);

            SalesText.Text = _sales.ToString("0.00");
            DepositsText.Text = _deposits.ToString("0.00");
            OutText.Text = _out.ToString("0.00");
        }

        private decimal Sum(string where)
        {
            object o = Data.Scalar(
                "SELECT COALESCE(SUM(price),0) FROM close_shift WHERE " + where +
                " AND ShiftNumber=@s AND Userid=@u;", ("@s", _shift), ("@u", _userId));
            return o == null || o == DBNull.Value ? 0m : Convert.ToDecimal(o);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Snapshot: sales and cash-out kept separate; profit = sales − expenses/withdrawals.
                Data.Execute(
                    "INSERT INTO shift_details (ShiftNum, TotalSell, TotalBuy, ProfitShift) VALUES (@sh,@ts,@tb,@p);",
                    ("@sh", _shift), ("@ts", _sales), ("@tb", _out), ("@p", _sales - _out));
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
