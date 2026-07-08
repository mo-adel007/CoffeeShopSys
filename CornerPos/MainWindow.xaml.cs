using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace CornerPos
{
    /// <summary>
    /// Application shell shown after login: espresso sidebar navigation, a header,
    /// and a content host into which each screen is swapped. The Cashier screen is
    /// live; the remaining screens show a placeholder until they are ported.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int _userId;
        private readonly string _userName;
        private readonly string _role;
        private readonly int _shift;
        private readonly DateTime _loginTime;
        private CashierView _cashier;
        private string _mode = "cashier"; // "admin" or "cashier"

        public MainWindow(int userId, string userName, string role, int shift, DateTime loginTime)
        {
            InitializeComponent();
            _userId = userId;
            _userName = userName ?? "";
            _role = role ?? "";
            _shift = shift;
            _loginTime = loginTime;

            UserName.Text = string.IsNullOrEmpty(_userName) ? "مستخدم" : _userName;
            bool isAdmin = string.Equals(_role, "admin", StringComparison.OrdinalIgnoreCase);
            UserRole.Text = isAdmin ? "مدير" : "كاشير";

            // Admins land in Admin mode and can toggle to a focused Cashier view.
            // Cashiers only ever see the Cashier-mode pages (no toggle).
            ModeToggle.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            SetMode(isAdmin ? "admin" : "cashier");

            SourceInitialized += (s, e) => TintTitleBar();
        }

        private void SetMode(string mode)
        {
            _mode = mode;
            bool admin = mode == "admin";
            AdminNav.Visibility = admin ? Visibility.Visible : Visibility.Collapsed;
            CashierNav.Visibility = admin ? Visibility.Collapsed : Visibility.Visible;
            ModeToggle.Content = admin ? "☕  التبديل إلى الكاشير" : "⚙  التبديل إلى الإدارة";

            if (admin) GoTo("Products", "المنتجات", "إدارة القائمة والأسعار والمخزون");
            else GoTo("Cashier", "الكاشير", "استقبال الطلبات وإدارة الوردية الحالية");
        }

        private void ModeToggle_Click(object sender, RoutedEventArgs e)
        {
            SetMode(_mode == "admin" ? "cashier" : "admin");
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            string key = Convert.ToString(btn.Tag);
            switch (key)
            {
                case "Cashier":         GoTo(key, "الكاشير", "استقبال الطلبات وإدارة الوردية الحالية"); break;
                case "Daily Movement":  GoTo(key, "الحركة اليومية", "تسجيل الإيداعات والمسحوبات والمصروفات"); break;
                case "Close Shift":     GoTo(key, "تقفيل الوردية", "مراجعة وتقفيل الوردية الحالية"); break;
                case "Close Day":       GoTo(key, "تقفيل اليوم", "مراجعة وتقفيل اليوم"); break;
                case "Stock":           GoTo(key, "المخزن", "إعادة تعبئة المخزون"); break;
                case "Products":        GoTo(key, "المنتجات", "إدارة القائمة والأسعار والمخزون"); break;
                case "Product Types":   GoTo(key, "الأصناف", "إدارة أصناف المنتجات"); break;
                case "Sales & Reports": GoTo(key, "التقارير", "المبيعات اليومية والشهرية"); break;
                case "Expenses":        GoTo(key, "المصروفات الشهرية", "تسجيل ومراجعة المصروفات الشهرية"); break;
                case "Users":           GoTo(key, "المستخدمون", "إدارة حسابات الموظفين"); break;
                case "Close Month":     GoTo(key, "تقفيل الشهر", "مراجعة وتقفيل الشهر"); break;
                default:                GoTo(key, key, ""); break;
            }
        }

        private void GoTo(string key, string title, string subtitle)
        {
            PageTitle.Text = title;
            PageSubtitle.Text = subtitle;

            switch (key)
            {
                case "Cashier":
                    if (_cashier == null) _cashier = new CashierView(_userId, _userName, _shift, _loginTime);
                    PageHost.Content = _cashier;
                    break;
                case "Products":
                    PageHost.Content = new ProductsView();
                    break;
                case "Product Types":
                    PageHost.Content = new ProductTypesView();
                    break;
                case "Users":
                    PageHost.Content = new UsersView();
                    break;
                case "Expenses":
                    PageHost.Content = new MonthlyExpensesView();
                    break;
                case "Daily Movement":
                    PageHost.Content = new DailyMovementView(_userId, _userName, _shift);
                    break;
                case "Close Shift":
                    PageHost.Content = new CloseShiftView(_shift, _userId);
                    break;
                case "Close Day":
                    PageHost.Content = new CloseDayView();
                    break;
                case "Stock":
                    PageHost.Content = new StockView();
                    break;
                case "Close Month":
                    PageHost.Content = new CloseMonthView();
                    break;
                case "Sales & Reports":
                    PageHost.Content = new ReportsView();
                    break;
                default:
                    PageHost.Content = Placeholder(title);
                    break;
            }
        }

        private UIElement Placeholder(string title)
        {
            var stack = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            stack.Children.Add(new TextBlock
            {
                Text = "☕",
                FontSize = 52,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = (Brush)FindResource("CaramelBrush")
            });
            stack.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 14, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = (Brush)FindResource("TextBrush")
            });
            stack.Children.Add(new TextBlock
            {
                Text = "هذه الشاشة قيد الإنشاء.",
                FontSize = 13,
                Margin = new Thickness(0, 6, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = (Brush)FindResource("MutedBrush")
            });
            return new Border
            {
                Margin = new Thickness(28),
                CornerRadius = new CornerRadius(16),
                Background = (Brush)FindResource("CardBrush"),
                BorderBrush = (Brush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(1),
                Child = stack
            };
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            // In-app renewal: enter a new key before the current one lapses.
            new ActivationWindow(isGate: false) { Owner = this }.ShowDialog();
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

        private static int ToColorRef(Color c) => (c.B << 16) | (c.G << 8) | c.R;
    }
}
