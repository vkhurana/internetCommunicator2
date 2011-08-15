using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace InterComm.InternalCommands
{
    class ProcessWrapper
    {
        public enum ProcessState
        {
            Starting,
            Running,
            Closing,
            Exited
        }

        private string fileName;
        private string startArgs;

        private int internalPID;
        private Process process;
        private ProcessState state;
        Timer timeoutCheckTimer;
        int timeout = 30; //seconds

        public ProcessWrapper(string filename, string arguments)
        {
            this.fileName = filename;
            this.startArgs = arguments;
            this.internalPID = -1;

            process = new Process();
            process.StartInfo.Arguments = arguments;
            process.StartInfo.FileName = filename;
            process.EnableRaisingEvents = true;
            
            //setup handler for when the process exits
            process.Exited += new EventHandler(process_Exited);
            state = ProcessState.Starting;
        }

        public int InternalPID
        {
            get
            {
                return internalPID;
            }
            set
            {
                internalPID = value;
            }
        }

        public void Start()
        {
            process.Start();
            state = ProcessState.Running;
        }

        public void Close()
        {
            process.Close();
            state = ProcessState.Closing;

            timeoutCheckTimer = new Timer(timeout * 1000);
            timeoutCheckTimer.Enabled = true;
            timeoutCheckTimer.Elapsed += new ElapsedEventHandler(timeoutCheckTimer_Elapsed);
        }

        void timeoutCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (process.HasExited == false)
            {
                process.Kill();
            }
            timeoutCheckTimer.Enabled = false;
        }

        private void process_Exited(object sender, EventArgs e)
        {
            state = ProcessState.Exited;
        }
    }
}
