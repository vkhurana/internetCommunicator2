using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

/*
 * service start <servicename>
 * service stop <servicename>
 * sarvice status <servicename>
 * 
 */

namespace InterComm.InternalCommands
{
    class Service : CommandBase
    {

        private string invalidCommandString = "Invalid service command\nUsage: service <start|stop|restart|status> <service name>";
        ServiceHandler serviceController;

        public Service(BotEngine engine)
            : base(engine)
        {
            serviceController = new ServiceHandler();
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
                return "service";
            }
        }

        public override void ProcessCommand(CommandDescriptor cmd)
        {
            string[] splitArgs = cmd.args.Split(new char[] { ' ' });
            string command = "null";
            string serviceName = "null";
            if (splitArgs.Length < 2)
            {
                SendReply(cmd, invalidCommandString);
                return;
            }

            command = splitArgs[0];
            command = command.ToLower();

            serviceName = string.Join(" ", splitArgs, 1, splitArgs.Length - 1);
            //serviceName = serviceName.ToLower();

            //SendReply(cmd, "command = " + command + "\nservicename = " + serviceName);
            string replyString = "null";

            switch (command)
            {
                case "start":
                    {
                        bool result = serviceController.StartService(serviceName);
                        replyString = result == true ? "Service (" + serviceName + ") " + command + "ed." : serviceController.LastError;
                        break;
                    }
                case "stop":
                    {
                        bool result = serviceController.StopService(serviceName);
                        replyString = result == true ? "Service (" + serviceName + ") " + command + "ped." : serviceController.LastError;
                        break;
                    }
                case "restart":
                    {
                        bool result = serviceController.RestartService(serviceName);
                        replyString = result == true ? "Service (" + serviceName + ") " + command + "ed." : serviceController.LastError;
                        break;
                    }
                case "status":
                    {
                        replyString = serviceController.GetStatus(serviceName);
                        break;
                    }
                default:
                    {
                        replyString = invalidCommandString;
                        break;
                    }
            }

            SendReply(cmd, replyString);
            return;
        }

    }
}
