using System;
using System.Windows.Forms;

namespace TravisCiMon
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += Application_ApplicationExit;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new TravisCiMonForm());
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            TravisCiMonForm.exitApp();
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            TravisCiMonForm.exitApp();
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            TravisCiMonForm.exitApp();
        }
    }
}
