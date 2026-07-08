using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Admin Stock screen (ports worker "Stock" tab). A read-only grid of products and
    /// their current stock on the left; on the right, pick a product and an amount to
    /// restock. Adding increments the product's Store column (COALESCE-guarded so a null
    /// stock starts from 0), parameterized so nothing here is injectable.
    /// </summary>
    public partial class StockView : UserControl
    {
        public StockView()
        {
            InitializeComponent();
            LoadProducts();
            LoadGrid();
        }

        private void LoadProducts()
        {
            var names = new List<string>();
            var dt = Data.Query("SELECT Product_name FROM product ORDER BY Product_name;");
            foreach (DataRow r in dt.Rows)
                names.Add(Convert.ToString(r["Product_name"]));
            ProductBox.ItemsSource = names;
        }

        private void LoadGrid()
        {
            Grid.ItemsSource = Data.Query(
                "SELECT Product_name, Store FROM product ORDER BY Product_name;").DefaultView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Error.Text = "";

            string productName = ProductBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(productName)) { Error.Text = "اختر منتجاً."; return; }

            if (!int.TryParse((AmountBox.Text ?? "").Trim(), out int amt) || amt <= 0)
            { Error.Text = "المبلغ يجب أن يكون رقماً صحيحاً موجباً."; return; }

            try
            {
                Data.Execute(
                    "UPDATE product SET Store = COALESCE(Store,0) + @amt WHERE Product_name = @n;",
                    ("@amt", amt), ("@n", productName));

                LoadGrid();
                AmountBox.Text = "";
                MessageBox.Show("تم تحديث المخزون.", "Corner", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { Error.Text = ex.Message; }
        }
    }
}
