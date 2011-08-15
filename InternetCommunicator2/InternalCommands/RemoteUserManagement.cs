using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterComm.Helpers;

namespace InterComm.InternalCommands
{
    class RemoteUserManagement : CommandBase
    {
        public RemoteUserManagement(BotEngine engine)
            : base(engine)
        {
            //constructor
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
                return "user";
            }
        }

        //bug!!! this needs to be called twice for add/remove to work
        public override void ProcessCommand(CommandDescriptor cmd)
        {
            string[] splitCommand = cmd.args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string command = splitCommand.Length >= 1 ? splitCommand[0] : "invalid";
            string username = splitCommand.Length >= 2 ? splitCommand[1] : "nouser";
            string nickname = splitCommand.Length >= 3 ? splitCommand[2] : null;
            bool result = false;

            string replystring = "";
            switch (command)
            {
                case "add":
                    {
                        try
                        {
                            result = iEngine.AddUser(username, nickname);
                        }
                        catch(Exception ex)
                        {
                            replystring = ex.Message + "\n";
                            result = false;
                        }
                        replystring += result ? "User successfully added!" : "Error adding user!";
                        break;
                    }
                case "remove":
                    {
                        try
                        {
                            result = iEngine.RemoveUser(username);
                        }
                        catch (Exception ex)
                        {
                            replystring = ex.Message + "\n";
                            result = false;
                        }
                        replystring += result ? "User successfully removed!" : "Error removing user!";
                        break;
                    }
                default:
                    {
                        replystring = "Invalid command parameter for user!\nUsage: User <add|remove> email [nickname]";
                        break;
                    }
            } //end switch
            SendReply(cmd, replystring);
            return;
        }
    }
}
