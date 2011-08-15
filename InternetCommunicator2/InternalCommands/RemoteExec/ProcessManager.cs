using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using InterComm.Helpers;

namespace InterComm.InternalCommands
{
    class ProcessManager
    {
        List<ProcessWrapper> processes;

        string lastError;
        public string LastError
        {
            get
            {
                return lastError;
            }
        }

        public ProcessManager()
        {
            processes = new List<ProcessWrapper>();
            lastError = "null";
        }

        public int StartAndEnqueue(string fileWithArgs)
        {
            int retval = -1;

            string[] seperated = Utilities.SeperateCommandAndArgs(fileWithArgs);
            try
            {
                ProcessWrapper processToStart = new ProcessWrapper(seperated[0], seperated[1]);
                processes.Add(processToStart);
                processToStart.Start();

                processToStart.InternalPID = processes.Count;

                retval = processToStart.InternalPID;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }

            return retval;
        }

        public bool Stop(string arguments)
        {
            bool retval = false;

            string[] args = Utilities.SeperateArguments(arguments);

            foreach (string arg in args)
            {
                if (arg.Length == 0) continue;

                int iPID = -1;
                try
                {
                    int.TryParse(arg, out iPID);
                    //if it reaches here, it means that the chap sent an iPID
                    //locate that iPID and kill it
                }
                catch
                {
                    //this means he sent us a process name... lets see if we can find that process
                    //find that process and kill it
                }
            }

            return retval;
        }
    }
}
