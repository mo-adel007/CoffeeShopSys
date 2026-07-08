using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CornerPos
{
    /// <summary>
    /// Admin Categories screen (ports Control Panel "product Type" tab). A grid of
    /// product types on the left; selecting a row loads it into the add/edit form on
    /// the right. Add / Update / Delete against the product_type table, parameterized.
    /// Names are unique — duplicates are blocked on both add and rename.
    /// </summary>
    public partial class ProductTypesView : UserControl
    {
        private int _selectedId; // 0 = adding a new category

        public ProductTypesView()
        {
            InitializeComponent();
            LoadGrid();
            ClearForm();
        }

        private void LoadGrid()
        {
            Grid.ItemsSource = Data.Query(
                "SELECT type_id, type_name FROM product_type ORDER BY type_name;").DefaultView;
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = Grid.SelectedItem as DataRowView;
            if (row == null) return;

            _selectedId = Convert.ToInt32(row["type_id"]);
            NameBox.Text = Convert.ToString(row["type_name"]);

            FormTitle.Text = "تعديل صنف";
            SaveBtn.Content = "حفظ التعديلات";
            DeleteBtn.IsEnabled = true;
            Error.Text = "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Error.Text = "";
            string name = (NameBox.Text ?? "").Trim();
            if (name.Length == 0) { Error.Text = "الاسم مطلوب."; return; }

            try
            {
                if (_selectedId == 0)
                {
                    // Guard against duplicate category names.
                    long dup = Convert.ToInt64(Data.Scalar(
                        "SELECT COUNT(*) FROM product_type WHERE type_name = @n;", ("@n", name)));
                    if (dup > 0) { Error.Text = "يوجد صنف بنفس الاسم."; return; }

                    Data.Execute(
                        "INSERT INTO product_type (type_name) VALUES (@n);", ("@n", name));
                }
                else
                {
                    // Block renaming onto another existing category's name.
                    long dup = Convert.ToInt64(Data.Scalar(
                        "SELECT COUNT(*) FROM product_type WHERE type_name = @n AND type_id <> @id;",
                        ("@n", name), ("@id", _selectedId)));
                    if (dup > 0) { Error.Text = "يوجد صنف بنفس الاسم."; return; }

                    Data.Execute(
                        "UPDATE product_type SET type_name=@n WHERE type_id=@id;",
                        ("@n", name), ("@id", _selectedId));
                }

                LoadGrid();
                ClearForm();
            }
            catch (Exception ex) { Error.Text = ex.Message; }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("حذف هذا الصنف؟", "Corner",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                Data.Execute("DELETE FROM product_type WHERE type_id=@id;", ("@id", _selectedId));
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
            FormTitle.Text = "إضافة صنف";
            SaveBtn.Content = "إضافة صنف";
            DeleteBtn.IsEnabled = false;
            Error.Text = "";
        }
    }
}
