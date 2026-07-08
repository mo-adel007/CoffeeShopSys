using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Admin Products screen (ports Control Panel "Products" tab). A grid of products
    /// on the left; selecting a row loads it into the add/edit form on the right.
    /// Add / Update / Delete against the product table, parameterized. Lets the owner
    /// set real prices and stock (replacing the seed placeholders).
    /// </summary>
    public partial class ProductsView : UserControl
    {
        private int _selectedId; // 0 = adding a new product

        public ProductsView()
        {
            InitializeComponent();
            LoadCategories();
            LoadGrid();
            ClearForm();
        }

        private void LoadCategories()
        {
            var cats = new List<Cat>();
            var dt = Data.Query("SELECT type_id, type_name FROM product_type ORDER BY type_name;");
            foreach (DataRow r in dt.Rows)
                cats.Add(new Cat { Id = Convert.ToInt32(r["type_id"]), Name = Convert.ToString(r["type_name"]) });
            CategoryBox.ItemsSource = cats;
        }

        private void LoadGrid()
        {
            Grid.ItemsSource = Data.Query(
                "SELECT p.Product_id, p.Product_name, p.Price, p.Store, p.ProductType_id, t.type_name " +
                "FROM product p LEFT JOIN product_type t ON t.type_id = p.ProductType_id " +
                "ORDER BY p.Product_name;").DefaultView;
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = Grid.SelectedItem as DataRowView;
            if (row == null) return;

            _selectedId = Convert.ToInt32(row["Product_id"]);
            NameBox.Text = Convert.ToString(row["Product_name"]);
            PriceBox.Text = row["Price"] == DBNull.Value ? "" : Convert.ToString(row["Price"]);
            StockBox.Text = row["Store"] == DBNull.Value ? "" : Convert.ToString(row["Store"]);
            CategoryBox.SelectedValue = row["ProductType_id"] == DBNull.Value ? null : (object)Convert.ToInt32(row["ProductType_id"]);

            FormTitle.Text = "تعديل منتج";
            SaveBtn.Content = "حفظ التعديلات";
            DeleteBtn.IsEnabled = true;
            Error.Text = "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Error.Text = "";
            string name = (NameBox.Text ?? "").Trim();
            if (name.Length == 0) { Error.Text = "الاسم مطلوب."; return; }

            decimal price = 0m;
            if (!string.IsNullOrWhiteSpace(PriceBox.Text) && !decimal.TryParse(PriceBox.Text.Trim(), out price))
            { Error.Text = "السعر يجب أن يكون رقماً."; return; }

            int stock = 0;
            if (!string.IsNullOrWhiteSpace(StockBox.Text) && !int.TryParse(StockBox.Text.Trim(), out stock))
            { Error.Text = "المخزون يجب أن يكون رقماً صحيحاً."; return; }

            if (CategoryBox.SelectedValue == null) { Error.Text = "اختر صنفاً."; return; }
            int typeId = Convert.ToInt32(CategoryBox.SelectedValue);

            try
            {
                if (_selectedId == 0)
                {
                    // Guard against duplicate product names.
                    long dup = Convert.ToInt64(Data.Scalar(
                        "SELECT COUNT(*) FROM product WHERE Product_name = @n;", ("@n", name)));
                    if (dup > 0) { Error.Text = "يوجد منتج بنفس الاسم."; return; }

                    Data.Execute(
                        "INSERT INTO product (Product_name, Price, ProductType_id, Store, MakeAButton) " +
                        "VALUES (@n, @p, @t, @s, 1);",
                        ("@n", name), ("@p", price), ("@t", typeId), ("@s", stock));
                }
                else
                {
                    Data.Execute(
                        "UPDATE product SET Product_name=@n, Price=@p, ProductType_id=@t, Store=@s WHERE Product_id=@id;",
                        ("@n", name), ("@p", price), ("@t", typeId), ("@s", stock), ("@id", _selectedId));
                }

                LoadGrid();
                ClearForm();
            }
            catch (Exception ex) { Error.Text = ex.Message; }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("حذف هذا المنتج؟", "Corner",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                Data.Execute("DELETE FROM product WHERE Product_id=@id;", ("@id", _selectedId));
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
            StockBox.Text = "";
            CategoryBox.SelectedIndex = -1;
            FormTitle.Text = "إضافة منتج";
            SaveBtn.Content = "إضافة منتج";
            DeleteBtn.IsEnabled = false;
            Error.Text = "";
        }

        private class Cat { public int Id { get; set; } public string Name { get; set; } }
    }
}
