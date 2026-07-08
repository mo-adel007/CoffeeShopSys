using System.Windows;
using CornerPos.Licensing;

namespace CornerPos
{
    /// <summary>
    /// Dual-purpose licensing window.
    ///
    /// As the startup <b>gate</b> (<c>isGate = true</c>): shown when the trial/key has
    /// expired. It IS the application's only window until a valid key is entered — the
    /// POS cannot be reached otherwise. Successful activation opens the login window;
    /// closing it exits the app.
    ///
    /// As an in-app <b>renewal dialog</b> (<c>isGate = false</c>): opened from the main
    /// window so the owner can enter a new key early (before lockout). It just closes on
    /// success/cancel and leaves the running session untouched.
    /// </summary>
    public partial class ActivationWindow : Window
    {
        private readonly bool _isGate;
        private bool _activated;

        public ActivationWindow(bool isGate)
        {
            InitializeComponent();
            _isGate = isGate;
            SecondaryBtn.Content = isGate ? "خروج" : "إغلاق";
            ShowStatus(LicenseManager.Evaluate());
        }

        private void ShowStatus(LicenseStatus s)
        {
            switch (s.State)
            {
                case LicenseState.Licensed:
                    StatusText.Text = "الترخيص مُفعّل.";
                    break;
                case LicenseState.Trial:
                    StatusText.Text = "النسخة التجريبية — متبقٍّ " + s.DaysLeft + " يوم";
                    break;
                default:
                    StatusText.Text = "انتهت الفترة التجريبية. أدخل مفتاح الترخيص للمتابعة.";
                    break;
            }
        }

        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            Message.Text = "";
            string error;
            if (!LicenseManager.TryActivate(KeyBox.Password, out error))
            {
                Message.Text = error;
                return;
            }

            _activated = true;

            if (_isGate)
            {
                new LoginWindow().Show();
                Close();
            }
            else
            {
                MessageBox.Show(this, "تم تفعيل الترخيص بنجاح.", "الترخيص",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
        }

        private void Secondary_Click(object sender, RoutedEventArgs e)
        {
            if (_isGate) Application.Current.Shutdown();
            else Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            // Closing the gate window without activating means the app can't run.
            if (_isGate && !_activated)
                Application.Current.Shutdown();
        }
    }
}
