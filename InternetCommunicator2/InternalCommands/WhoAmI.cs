using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterComm.InternalCommands
{
    class WhoAmI : CommandBase
    {
        public WhoAmI(BotEngine engine)
            : base(engine)
        {

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
                return "whoami";
            }
        }

        public override void ProcessCommand(CommandDescriptor cmd)
        {
            StringBuilder replyString = new StringBuilder();
            replyString.Append("\nUsername = " + cmd.fromUser.UserName);
            replyString.Append("\nNickname = " + cmd.fromUser.NickName);
            replyString.Append("\nAuthLevel = " + cmd.fromUser.AuthLevel);
            replyString.Append("\nState = " + cmd.fromUser.State);
            SendReply(cmd, replyString.ToString());
            return;
        }
    }
}
