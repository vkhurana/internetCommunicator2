using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace InterComm.InternalCommands
{
    class SystemFunctions
    {
        [DllImport("user32.dll")]
        public static extern void LockWorkStation();

        [DllImport("user32.dll")]
        public static extern int ExitWindowsEx(int uFlags, int dwReason);

        private string lastError = "null";

        public string LastError
        {
            get
            {
                return lastError;
            }
        }

        public bool Lock()
        {
            bool result = false;

            try
            {
                LockWorkStation();
                result = true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                result = false;
            }

            return result;
        }

        public bool LogOff()
        {
            bool result = false;

            try
            {
                ExitWindowsEx(4, 0); //4 is like a force, replace it with a 0 for regular logoff
                result = true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                result = false;
            }

            return result;
        }

        public bool Reboot()
        {
            bool result = false;

            try
            {
                Process.Start("shutdown", "-f -r -t 00");
                ExitWindowsEx(2, 0);
                result = true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                result = false;
            }

            return result;
        }

        public bool Shutdown()
        {
            bool result = false;

            try
            {
                Process.Start("shutdown", "-f -s -t 00");
                ExitWindowsEx(0x8 | 0x4, 0);
                result = true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                result = false;
            }

            return result;
        }

        public bool Hibernate()
        {
            bool result = false;

            try
            {
                Application.SetSuspendState(PowerState.Hibernate, true, false);
                result = true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                result = false;
            }

            return result;
        }

        public bool Standby()
        {
            bool result = false;

            try
            {
                Application.SetSuspendState(PowerState.Suspend, true, false);
                result = true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                result = false;
            }

            return result;
        }

    }
}
