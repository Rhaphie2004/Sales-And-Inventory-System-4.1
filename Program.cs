using sims.Admin_Side;
<<<<<<< HEAD
using sims.Admin_Side.Stocks;
=======
>>>>>>> 76ff571b76b30d47259db34b2a5e8d7493ef0d44
using sims.Splash_page_and_Loading_Screen;
using sims.Staff_Side;
using System;
using System.IO;
using System.Windows.Forms;

namespace sims
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
<<<<<<< HEAD
            Application.Run(new Dashboard_Staff());
=======

            // Add the database setup code here
            if (FirstTimeRun())
            {
                string dbName = "sims";
                string scriptPath = Path.Combine(Application.StartupPath, "sales and inventory system.sql");

                if (DatabaseSetup.SetupDatabase(dbName, scriptPath))
                {
                    MessageBox.Show("Database setup completed successfully!");
                }
            }

            Application.Run(new DashboardOwner());
>>>>>>> 76ff571b76b30d47259db34b2a5e8d7493ef0d44
        }

        // Add this method inside the Program class
        private static bool FirstTimeRun()
        {
            bool firstRun = Properties.Settings.Default.FirstRun;

            if (firstRun)
            {
                Properties.Settings.Default.FirstRun = false;
                Properties.Settings.Default.Save();
            }

            return firstRun;
        }
    }
}
