using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSDK;

namespace EchoPlugin
{
    [StatelessChatPluginAttribute("sends back the local time")]
    class TimePlugin : IStatelessChatInterface
    {
        public string GetCommandResult(string cmdArgs)
        {
            string timezone = cmdArgs;
            timezone = timezone.Trim();
            timezone = timezone.ToLower();
            string returnMessage = "null";

            switch (timezone)
            {
                case "ist":
                    {
                        break;
                    }
                default:
                    {
                        returnMessage = DateTime.Now.DayOfWeek + " " + DateTime.Now.ToString();
                        break;
                    }
            }

            return returnMessage;
        }
        public string GetCommandTrigger()
        {
            return "time";
        }
    }
}
