using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Corner_Application
{
    static class Program
    {
        /// <summary>
        /// Connection string used by every form. Now points at the portable, embedded
        /// SQLite database (corner.db, next to the EXE) instead of a local MySQL server —
        /// no XAMPP / MySQL install is required to store data. See <see cref="Db"/>.
        /// </summary>
        public static string Constring = Db.ConnectionString();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()

        {
            // Create the database file, schema and seed admin on first run (idempotent).
            try
            {
                Db.EnsureCreated();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not initialise the local database:\n\n" + ex.Message,
                    "Corner Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            //Application.Run(new order());
            //Application.Run(new worker());
        }
    }
}
