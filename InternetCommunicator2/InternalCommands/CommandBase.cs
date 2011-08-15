using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterComm.InternalCommands
{
    abstract class CommandBase
    {
        protected BotEngine iEngine;
        public CommandBase(BotEngine engine)
        {
            iEngine = engine;
        }

        public abstract string CommandTrigger
        {
            get;
        }
        
        public abstract void ProcessCommand(CommandDescriptor cmd);

        public abstract UserAuthLevel MinAuthLevel
        {
            get;
        }

        protected void SendReply(CommandDescriptor cmd, string message)
        {
            iEngine.SendMessage(cmd.fromUser, message);
        }

    }
}
