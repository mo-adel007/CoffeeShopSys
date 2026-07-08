using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Reports / Search (ports Control Panel "Search" tab), reimplemented correctly
    /// (the legacy grouped-sum query crashed on a malformed column). Four modes over a
    /// date range: by product type, by product, details (sales + safe deposits/
    /// withdrawals), and monthly expenses. Deposits are keyed on safe.Type = 'Deposit',
    /// matching what the Daily Movement screen writes.
    /// </summary>
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
            TypeBox.SelectedIndex = 0;
            FromDate.SelectedDate = DateTime.Today.AddDays(-30);
            ToDate.SelectedDate = DateTime.Today;
        }

        private string Mode => (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        private void Type_Changed(object sender, SelectionChangedEventArgs e)
        {
            switch (Mode)
            {
                case "By product type":
                    NamePanel.Visibility = Visibility.Visible;
                    NameLbl.Text = "CATEGORY";
                    NameBox.ItemsSource = LoadNames("SELECT type_id AS Id, type_name AS Name FROM product_type ORDER BY type_name;", false);
                    break;
                case "By product":
                    NamePanel.Visibility = Visibility.Visible;
                    NameLbl.Text = "PRODUCT";
                    NameBox.ItemsSource = LoadNames("SELECT Product_id AS Id, Product_name AS Name FROM product ORDER BY Product_name;", true);
                    break;
                default: // Details / Monthly expenses — no name picker
                    NamePanel.Visibility = Visibility.Collapsed;
                    break;
            }
            if (NameBox.Items.Count > 0) NameBox.SelectedIndex = 0;
        }

        private List<Item> LoadNames(string sql, bool withAll)
        {
            var list = new List<Item>();
            if (withAll) list.Add(new Item { Id = 0, Name = "All products" });
            foreach (DataRow r in Data.Query(sql).Rows)
                list.Add(new Item { Id = Convert.ToInt32(r["Id"]), Name = Convert.ToString(r["Name"]) });
            return list;
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            if (FromDate.SelectedDate == null || ToDate.SelectedDate == null)
            { Summary.Text = "Pick a date range."; return; }

            string from = FromDate.SelectedDate.Value.ToString("yyyy-MM-ddT00:00:00");
            string to = ToDate.SelectedDate.Value.ToString("yyyy-MM-ddT23:59:59");

            try
            {
                switch (Mode)
                {
                    case "By product type": RunByType(from, to); break;
                    case "By product": RunByProduct(from, to); break;
                    case "Details (sales + safe)": RunDetails(from, to); break;
                    default: RunMonthly(from, to); break;
                }
            }
            catch (Exception ex) { Summary.Text = ex.Message; }
        }

        private void RunByType(string from, string to)
        {
            int typeId = NameBox.SelectedValue == null ? 0 : Convert.ToInt32(NameBox.SelectedValue);
            var dt = Data.Query(
                "SELECT pp.Product_name AS Product, pp.User_Name AS Cashier, SUM(pp.quantity) AS Qty, SUM(pp.price) AS Total " +
                "FROM product_process pp JOIN product p ON p.Product_id = pp.IdProduct " +
                "WHERE p.ProductType_id=@t AND pp.Process_type='sell' AND pp.\"DateTime\" BETWEEN @from AND @to " +
                "GROUP BY pp.Product_name, pp.User_Name ORDER BY Total DESC;",
                ("@t", typeId), ("@from", from), ("@to", to));
            Grid.ItemsSource = dt.DefaultView;
            Summary.Text = "Total sales: " + Sum(dt, "Total").ToString("0.00");
        }

        private void RunByProduct(string from, string to)
        {
            int productId = NameBox.SelectedValue == null ? 0 : Convert.ToInt32(NameBox.SelectedValue);
            DataTable dt;
            if (productId == 0)
                dt = Data.Query(
                    "SELECT Product_name AS Product, User_Name AS Cashier, SUM(quantity) AS Qty, SUM(price) AS Total " +
                    "FROM product_process WHERE Process_type='sell' AND \"DateTime\" BETWEEN @from AND @to " +
                    "GROUP BY Product_name, User_Name ORDER BY Total DESC;",
                    ("@from", from), ("@to", to));
            else
                dt = Data.Query(
                    "SELECT Product_name AS Product, User_Name AS Cashier, SUM(quantity) AS Qty, SUM(price) AS Total " +
                    "FROM product_process WHERE Process_type='sell' AND IdProduct=@pid AND \"DateTime\" BETWEEN @from AND @to " +
                    "GROUP BY Product_name, User_Name;",
                    ("@pid", productId), ("@from", from), ("@to", to));
            Grid.ItemsSource = dt.DefaultView;
            Summary.Text = "Total sales: " + Sum(dt, "Total").ToString("0.00");
        }

        private void RunDetails(string from, string to)
        {
            var dt = Data.Query(
                "SELECT Product_name AS Product, User_Name AS Cashier, SUM(quantity) AS Qty, SUM(price) AS Total " +
                "FROM product_process WHERE Process_type='sell' AND \"DateTime\" BETWEEN @from AND @to " +
                "GROUP BY Product_name, User_Name ORDER BY Total DESC;",
                ("@from", from), ("@to", to));
            Grid.ItemsSource = dt.DefaultView;

            decimal deposit = ScalarSum(
                "SELECT SUM(price) FROM safe WHERE Type='Deposit' AND Dtime BETWEEN @from AND @to;", from, to);
            decimal withdraw = ScalarSum(
                "SELECT SUM(price) FROM safe WHERE Type<>'Deposit' AND Dtime BETWEEN @from AND @to;", from, to);
            decimal sales = Sum(dt, "Total");
            Summary.Text = string.Format(
                "Sales: {0:0.00}    Deposits: {1:0.00}    Withdrawals/expenses: {2:0.00}    Net (deposits − withdrawals): {3:0.00}",
                sales, deposit, withdraw, deposit - withdraw);
        }

        private void RunMonthly(string from, string to)
        {
            var dt = Data.Query(
                "SELECT expens_name AS Expense, Price FROM all_monthlyexpenses " +
                "WHERE Date_time BETWEEN @from AND @to ORDER BY Date_time;",
                ("@from", from), ("@to", to));
            Grid.ItemsSource = dt.DefaultView;
            Summary.Text = "Total monthly expenses: " + Sum(dt, "Price").ToString("0.00");
        }

        private decimal ScalarSum(string sql, string from, string to)
        {
            object o = Data.Scalar(sql, ("@from", from), ("@to", to));
            return o == null || o == DBNull.Value ? 0m : Convert.ToDecimal(o);
        }

        private static decimal Sum(DataTable dt, string col)
        {
            decimal s = 0m;
            foreach (DataRow r in dt.Rows)
                if (r[col] != DBNull.Value) s += Convert.ToDecimal(r[col]);
            return s;
        }

        private class Item { public int Id { get; set; } public string Name { get; set; } }
    }
}
