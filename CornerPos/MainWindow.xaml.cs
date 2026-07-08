using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CornerPos
{
    /// <summary>
    /// Application shell shown after login: espresso sidebar navigation, a header,
    /// and a content area. The individual screens (cashier, products, reports…) are
    /// being ported into this shell; for now each shows a placeholder so the layout
    /// and navigation can be reviewed.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _role;

        public MainWindow(string userName, string role)
        {
            InitializeComponent();
            _role = role ?? "";

            UserName.Text = string.IsNullOrEmpty(userName) ? "User" : userName;
            bool isAdmin = string.Equals(_role, "admin", StringComparison.OrdinalIgnoreCase);
            UserRole.Text = isAdmin ? "Administrator" : "Cashier";

            // A cashier only needs the till; hide the back-office sections.
            if (!isAdmin)
            {
                NavProducts.Visibility = Visibility.Collapsed;
                NavSales.Visibility = Visibility.Collapsed;
                NavExpenses.Visibility = Visibility.Collapsed;
                NavUsers.Visibility = Visibility.Collapsed;
            }

            ShowPage("Cashier", "\U0001F4B3", "Take orders and manage the current shift");

            SourceInitialized += (s, e) => TintTitleBar();
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn == null) return;
            string page = Convert.ToString(btn.Tag);
            switch (page)
            {
                case "Cashier":            ShowPage(page, "\U0001F4B3", "Take orders and manage the current shift"); break;
                case "Products":           ShowPage(page, "☕", "Manage the menu, prices and stock"); break;
                case "Sales & Reports":    ShowPage(page, "\U0001F4C8", "Daily, shift and monthly sales at a glance"); break;
                case "Expenses":           ShowPage(page, "\U0001F4B0", "Record and review shop expenses"); break;
                case "Users":              ShowPage(page, "\U0001F465", "Manage staff logins and access"); break;
                default:                   ShowPage(page, "☕", ""); break;
            }
        }

        private void ShowPage(string title, string icon, string subtitle)
        {
            PageTitle.Text = title;
            PageSubtitle.Text = subtitle;
            PageIcon.Text = icon;
            PageBody.Text = title + " screen";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        // ---- Windows 11 espresso title bar (best-effort) ----
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        private void TintTitleBar()
        {
            try
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                int caption = ToColorRef(Color.FromRgb(0x3E, 0x27, 0x23));
                int text = ToColorRef(Color.FromRgb(0xF5, 0xEF, 0xE6));
                DwmSetWindowAttribute(hwnd, DWMWA_CAPTION_COLOR, ref caption, sizeof(int));
                DwmSetWindowAttribute(hwnd, DWMWA_TEXT_COLOR, ref text, sizeof(int));
            }
            catch { /* pre-Windows 11: ignore */ }
        }

        private static int ToColorRef(Color c)
        {
            return (c.B << 16) | (c.G << 8) | c.R; // 0x00BBGGRR
        }
    }
}
