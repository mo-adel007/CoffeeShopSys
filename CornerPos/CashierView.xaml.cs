using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Corner_Application; // shared Db

namespace CornerPos
{
    /// <summary>
    /// Cashier / point-of-sale screen. Shows the menu as tiles (filterable by
    /// category), builds an order in memory, and on "Charge" records the sale into
    /// close_shift + close_day + product_process and decrements stock — the same
    /// tables the admin reports read, so nothing downstream changes.
    /// </summary>
    public partial class CashierView : UserControl
    {
        private readonly int _userId;
        private readonly string _userName;
        private readonly int _shift;
        private readonly ObservableCollection<CartItem> _cart = new ObservableCollection<CartItem>();

        public CashierView(int userId, string userName, int shift)
        {
            InitializeComponent();
            _userId = userId;
            _userName = userName ?? "";
            _shift = shift;

            CartList.ItemsSource = _cart;
            _cart.CollectionChanged += (s, e) => Refresh();

            LoadCategories();
            LoadProducts(0);
            Refresh();
        }

        // ---------- data loading ----------
        private void LoadCategories()
        {
            var cats = new List<Category> { new Category { Id = 0, Name = "All" } };
            try
            {
                using (var conn = new SQLiteConnection(Db.ConnectionString()))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT type_id, type_name FROM product_type ORDER BY type_id;", conn))
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            cats.Add(new Category { Id = Convert.ToInt32(r["type_id"]), Name = Convert.ToString(r["type_name"]) });
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Corner", MessageBoxButton.OK, MessageBoxImage.Error); }
            CategoryList.ItemsSource = cats;
        }

        private void LoadProducts(int typeId)
        {
            var products = new List<Product>();
            try
            {
                using (var conn = new SQLiteConnection(Db.ConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT Product_id, Product_name, Price, Store, ProductType_id FROM product";
                    if (typeId != 0) sql += " WHERE ProductType_id = @t";
                    sql += " ORDER BY Product_name;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        if (typeId != 0) cmd.Parameters.AddWithValue("@t", typeId);
                        using (var r = cmd.ExecuteReader())
                            while (r.Read())
                                products.Add(new Product
                                {
                                    Id = Convert.ToInt32(r["Product_id"]),
                                    Name = Convert.ToString(r["Product_name"]),
                                    Price = r["Price"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Price"]),
                                    Store = r["Store"] == DBNull.Value ? 0 : Convert.ToInt32(r["Store"])
                                });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Corner", MessageBoxButton.OK, MessageBoxImage.Error); }
            ProductList.ItemsSource = products;
        }

        // ---------- interactions ----------
        private void Category_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(((Button)sender).Tag);
            LoadProducts(id);
        }

        private void Product_Click(object sender, RoutedEventArgs e)
        {
            var p = (Product)((Button)sender).Tag;
            var existing = _cart.FirstOrDefault(c => c.ProductId == p.Id);
            if (existing != null) existing.Qty++;
            else _cart.Add(new CartItem { ProductId = p.Id, Name = p.Name, UnitPrice = p.Price, Qty = 1 });
            Refresh();
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            var item = (CartItem)((Button)sender).Tag;
            item.Qty++;
            Refresh();
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            var item = (CartItem)((Button)sender).Tag;
            item.Qty--;
            if (item.Qty <= 0) _cart.Remove(item);
            Refresh();
        }

        private void Clear_Click(object sender, RoutedEventArgs e) => _cart.Clear();

        private void Charge_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("The order is empty.", "Corner", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string now = DateTime.Now.ToString("s");
            try
            {
                using (var conn = new SQLiteConnection(Db.ConnectionString()))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        foreach (var item in _cart)
                        {
                            decimal line = item.LineTotal;

                            Exec(conn, "INSERT INTO close_shift (processT,price,proName,quantity,UserN,Dtime,Userid,ShiftNumber) " +
                                       "VALUES ('sell',@price,@name,@qty,@user,@dt,@uid,@shift);", item, line, now);
                            Exec(conn, "INSERT INTO close_day (processT,price,proName,quantity,UserN,Dtime,Userid,ShiftNumber) " +
                                       "VALUES ('sell',@price,@name,@qty,@user,@dt,@uid,@shift);", item, line, now);

                            using (var pp = new SQLiteCommand(
                                "INSERT INTO product_process (User_Name,Process_type,Product_name,quantity,price,\"DateTime\",IdProduct,UserId) " +
                                "VALUES (@user,'sell',@name,@qty,@price,@dt,@pid,@uid);", conn))
                            {
                                pp.Parameters.AddWithValue("@user", _userName);
                                pp.Parameters.AddWithValue("@name", item.Name);
                                pp.Parameters.AddWithValue("@qty", item.Qty);
                                pp.Parameters.AddWithValue("@price", line);
                                pp.Parameters.AddWithValue("@dt", now);
                                pp.Parameters.AddWithValue("@pid", item.ProductId);
                                pp.Parameters.AddWithValue("@uid", _userId);
                                pp.ExecuteNonQuery();
                            }

                            using (var up = new SQLiteCommand(
                                "UPDATE product SET Store = COALESCE(Store,0) - @qty WHERE Product_id = @pid;", conn))
                            {
                                up.Parameters.AddWithValue("@qty", item.Qty);
                                up.Parameters.AddWithValue("@pid", item.ProductId);
                                up.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();
                    }
                }

                MessageBox.Show("Order charged: " + Total().ToString("0.00"), "Corner",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _cart.Clear();
                LoadProducts(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Corner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exec(SQLiteConnection conn, string sql, CartItem item, decimal line, string now)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@price", line);
                cmd.Parameters.AddWithValue("@name", item.Name);
                cmd.Parameters.AddWithValue("@qty", item.Qty);
                cmd.Parameters.AddWithValue("@user", _userName);
                cmd.Parameters.AddWithValue("@dt", now);
                cmd.Parameters.AddWithValue("@uid", _userId);
                cmd.Parameters.AddWithValue("@shift", _shift);
                cmd.ExecuteNonQuery();
            }
        }

        private decimal Total() => _cart.Sum(c => c.LineTotal);

        private void Refresh()
        {
            TotalText.Text = Total().ToString("0.00");
            EmptyHint.Visibility = _cart.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // ---------- models ----------
        private class Category { public int Id { get; set; } public string Name { get; set; } }

        private class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Store { get; set; }
            public string PriceText => Price > 0 ? Price.ToString("0.00") : "—";
        }

        private class CartItem : INotifyPropertyChanged
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public decimal UnitPrice { get; set; }

            private int _qty;
            public int Qty
            {
                get => _qty;
                set { _qty = value; OnChanged(nameof(Qty)); OnChanged(nameof(LineText)); }
            }

            public decimal LineTotal => UnitPrice * Qty;
            public string LineText => Qty + " × " + UnitPrice.ToString("0.00") + " = " + LineTotal.ToString("0.00");

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnChanged(string p) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}
