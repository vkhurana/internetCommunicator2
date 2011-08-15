using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterComm.Helpers;

namespace InterComm.InternalCommands
{
    class RemoteExec : CommandBase
    {
        //proc start <file name> <= send back a iPID
        //proc details <iPID> | <process name>
        //proc stop <iPID> | <process name>
        //proc list <= iPID, details (cpu, mem, start time)
        //proc cleanup (management)

        private string CommandHelp = "Usage: proc <start|stop|list|cleanup>";
        ProcessManager procMan;

        public RemoteExec(BotEngine engine)
            : base(engine)
        {
            procMan = new ProcessManager();
        }

        public override UserAuthLevel MinAuthLevel
        {
            get
            {
                return UserAuthLevel.Regular;
            }
        }

        public override string CommandTrigger
        {
            get
            {
                return "proc";
            }
        }

        public override void ProcessCommand(CommandDescriptor cmd)
        {
            if (cmd.args.Length == 0)
            {
                SendReply(cmd, CommandHelp);
                return;
            }

            string[] splitCommand = Utilities.SeperateCommandAndArgs(cmd.args);//cmd.args.Trim().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            string command = "null";
            string arguments = "null";
            string returnString = "null";

            command = splitCommand[0];
            arguments = splitCommand[1];

            switch (command)
            {
                case "start":
                    {
                        int iPID = procMan.StartAndEnqueue(arguments);
                        returnString = iPID != -1 ? "Success" : "Error: " + procMan.LastError;
                        break;
                    }
                case "stop":
                    {
                        procMan.Stop(arguments);
                        break;
                    }
                case "details":
                    {
                        break;
                    }
                case "list":
                    {
                        break;
                    }
                case "cleanup":
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            //SendReply(cmd, "cmd = " + command + " args= " + arguments);
            SendReply(cmd, returnString);

            return;
        }
    }
}
