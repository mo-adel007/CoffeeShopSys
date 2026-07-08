using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Daily Movement (ports worker "Daily movement" tab): records safe operations —
    /// Deposit, Withdrawal, Buy goods, Other expense. Each entry fans out to three
    /// tables in one transaction: safe (full detail), close_shift and close_day
    /// (aggregation, processT = 'sell' for a deposit, else 'buy'). The operation label
    /// stored in safe.Type is what the Reports "Details" view keys on.
    /// </summary>
    public partial class DailyMovementView : UserControl
    {
        private readonly int _userId;
        private readonly string _userName;
        private readonly int _shift;

        public DailyMovementView(int userId, string userName, int shift)
        {
            InitializeComponent();
            _userId = userId;
            _userName = userName ?? "";
            _shift = shift;
            OpBox.SelectedIndex = 0;
        }

        private string Op => (OpBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";

        private void Op_Changed(object sender, SelectionChangedEventArgs e)
        {
            // Toggle fields + relabel the name box for the selected operation.
            switch (Op)
            {
                case "Deposit":
                    NameLabel.Text = "اسم المودع";
                    Show(ReasonPanel, true); Show(QtyPanel, false);
                    break;
                case "Withdrawal":
                    NameLabel.Text = "اسم الساحب";
                    Show(ReasonPanel, true); Show(QtyPanel, false);
                    break;
                case "Buy goods":
                    NameLabel.Text = "اسم الصنف";
                    Show(ReasonPanel, false); Show(QtyPanel, true);
                    break;
                default: // Other expense
                    NameLabel.Text = "اسم المصروف";
                    Show(ReasonPanel, false); Show(QtyPanel, false);
                    break;
            }
        }

        private static void Show(UIElement el, bool visible) =>
            el.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Error.Text = "";
            string op = Op;
            if (string.IsNullOrEmpty(op)) { Error.Text = "اختر نوع العملية."; return; }

            decimal amount;
            if (!decimal.TryParse((AmountBox.Text ?? "").Trim(), out amount) || amount <= 0)
            { Error.Text = "أدخل مبلغاً صحيحاً."; return; }

            string name = (NameBox.Text ?? "").Trim();

            // Map the operation to the shared column values (mirrors the legacy Ch* methods).
            string processT = op == "Deposit" ? "sell" : "buy";
            string reason = "", proName = "", reasonOfT = "", personTake = "", whoAdd = "";
            int qty = 0;

            switch (op)
            {
                case "Deposit":
                    whoAdd = name; reason = (ReasonBox.Text ?? "").Trim();
                    break;
                case "Withdrawal":
                    personTake = name; reason = (ReasonBox.Text ?? "").Trim();
                    break;
                case "Buy goods":
                    proName = name;
                    int.TryParse((QtyBox.Text ?? "").Trim(), out qty);
                    break;
                default: // Other expense
                    reasonOfT = name;
                    break;
            }

            string now = DateTime.Now.ToString("s");
            try
            {
                var statements = new List<(string, (string, object)[])>
                {
                    ("INSERT INTO safe (Type,price,reason,proName,quantity,ReasonOfT,PersonTake,UserN,Dtime,Userid,Who_personAdd) " +
                     "VALUES (@type,@price,@reason,@proName,@qty,@reasonOfT,@personTake,@userN,@dt,@uid,@whoAdd);",
                        new (string, object)[] {
                            ("@type", op), ("@price", amount), ("@reason", reason), ("@proName", proName),
                            ("@qty", qty), ("@reasonOfT", reasonOfT), ("@personTake", personTake),
                            ("@userN", _userName), ("@dt", now), ("@uid", _userId), ("@whoAdd", whoAdd) }),

                    ("INSERT INTO close_shift (processT,price,reason,proName,quantity,ReasonOfT,PersonTake,UserN,Dtime,Userid,ShiftNumber,WhoPersonAdd) " +
                     "VALUES (@processT,@price,@reason,@proName,@qty,@reasonOfT,@personTake,@userN,@dt,@uid,@shift,@whoAdd);",
                        new (string, object)[] {
                            ("@processT", processT), ("@price", amount), ("@reason", reason), ("@proName", proName),
                            ("@qty", qty), ("@reasonOfT", reasonOfT), ("@personTake", personTake),
                            ("@userN", _userName), ("@dt", now), ("@uid", _userId), ("@shift", _shift), ("@whoAdd", whoAdd) }),

                    ("INSERT INTO close_day (processT,price,reason,proName,quantity,ReasonOfT,PersonT,UserN,Dtime,Userid,ShiftNumber,whoPerson_add) " +
                     "VALUES (@processT,@price,@reason,@proName,@qty,@reasonOfT,@personTake,@userN,@dt,@uid,@shift,@whoAdd);",
                        new (string, object)[] {
                            ("@processT", processT), ("@price", amount), ("@reason", reason), ("@proName", proName),
                            ("@qty", qty), ("@reasonOfT", reasonOfT), ("@personTake", personTake),
                            ("@userN", _userName), ("@dt", now), ("@uid", _userId), ("@shift", _shift), ("@whoAdd", whoAdd) }),
                };

                Data.Batch(statements);

                Error.Text = "";
                MessageBox.Show("تم تسجيل الحركة.", "Corner", MessageBoxButton.OK, MessageBoxImage.Information);
                AmountBox.Text = ""; NameBox.Text = ""; ReasonBox.Text = ""; QtyBox.Text = "";
            }
            catch (Exception ex) { Error.Text = ex.Message; }
        }
    }
}
