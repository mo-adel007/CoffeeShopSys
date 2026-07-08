using System;
using System.Windows;
using Corner_Application; // shared Db + Security (linked from the WinForms project)

namespace CornerPos
{
    /// <summary>
    /// Application entry point. Creates the portable SQLite database on first run
    /// (same foundation as the WinForms build) and then shows the login window.
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

            new LoginWindow().Show();
        }
    }
}
