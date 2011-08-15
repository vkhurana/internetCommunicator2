using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterComm.InternalCommands
{
    class SystemController:CommandBase
    {
        
        SystemFunctions sysFuncs;

        public SystemController(BotEngine engine)
            : base(engine)
        {
            sysFuncs = new SystemFunctions();
        }

        public override UserAuthLevel MinAuthLevel
        {
            get
            {
                return UserAuthLevel.Admin;
            }
        }

        public override string CommandTrigger
        {
            get
            {
                return "system";
            }
        }

        public override void ProcessCommand(CommandDescriptor cmd)
        {
            string replyString = "null";

            switch (cmd.args)
            {
                case "shutdown":
                    {
                        bool result = sysFuncs.Shutdown();
                        replyString = result == true ? "Success" : "Unable to " + cmd.args + "(" + sysFuncs.LastError +")";
                        break;
                    }
                case "restart":
                    {
                        bool result = sysFuncs.Reboot();
                        replyString = result == true ? "Success" : "Unable to " + cmd.args + "(" + sysFuncs.LastError + ")";
                        break;
                    }
                case "hibernate":
                    {
                        bool result = sysFuncs.Hibernate();
                        replyString = result == true ? "Success" : "Unable to " + cmd.args + "(" + sysFuncs.LastError + ")";
                        break;
                    }
                case "standby":
                    {
                        bool result = sysFuncs.Standby();
                        replyString = result == true ? "Success" : "Unable to " + cmd.args + "(" + sysFuncs.LastError + ")";
                        break;
                    }
                case "logoff":
                    {
                        bool result = sysFuncs.LogOff();
                        replyString = result == true ? "Success" : "Unable to " + cmd.args + "(" + sysFuncs.LastError + ")";
                        break;
                    }
                case "lock":
                    {
                        bool result = sysFuncs.Lock();
                        replyString = result == true ? "Success" : "Unable to " + cmd.args + "(" + sysFuncs.LastError + ")";
                        break;
                    }
                default:
                    {
                        replyString = "Usage: " + CommandTrigger + "  <shutdown|restart|hibernate|standby|logoff|lock>";
                        break;
                    }
            }

            SendReply(cmd, replyString);
            return;
        }
    }
}
