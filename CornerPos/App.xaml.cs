using System;
using System.Windows;
using Corner_Application; // shared Db + Security (linked from the WinForms project)
using CornerPos.Licensing;

namespace CornerPos
{
    /// <summary>
    /// Application entry point. Creates the portable SQLite database on first run
    /// (same foundation as the WinForms build), enforces licensing (7-day trial then a
    /// signed key), and shows the login window.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Db.EnsureCreated();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not initialise the local database:\n\n" + ex.Message,
                    "Corner", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // License gate: an expired trial/key can only reach the activation window;
            // a valid trial or key proceeds to login (with a warning if expiring soon).
            LicenseStatus license = LicenseManager.Evaluate();
            if (!license.CanUseApp)
            {
                new ActivationWindow(isGate: true).Show();
                return;
            }

            new LoginWindow().Show();

            if (license.WarnSoon)
                MessageBox.Show(
                    "ينتهي الترخيص خلال " + license.DaysLeft + " يوم. برجاء التجديد لتفادي التوقف.",
                    "الترخيص", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
