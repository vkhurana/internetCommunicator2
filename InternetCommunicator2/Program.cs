using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InterComm.Helpers;

namespace InterComm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();
            //swallow it!
            //todo(0) - do sometihng!
            Logger.LogException((Exception)e.ExceptionObject);
        }
    }
}
