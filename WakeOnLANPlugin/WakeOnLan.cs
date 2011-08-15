using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSDK;

namespace WakeOnLANPlugin
{
    [StatelessChatPluginAttribute("wake up a computer on the local lan :)")]
    public class WakeOnLan: IStatelessChatInterface
    {
        public string GetCommandResult(string cmdArgs)
        {
            string macAddress = cmdArgs;

            WOLHandler wolHandler = new WOLHandler();

            if (string.Equals(cmdArgs, "raptor", StringComparison.InvariantCultureIgnoreCase))
            {
                macAddress = "00-00-00-00-00-00";
            }

            bool result = wolHandler.wakeUpMac(macAddress);

            return result == true ? "Success" : "ERROR! " + wolHandler.LastError;
        }

        public string GetCommandTrigger()
        {
            return "wol";
        }
    }
}
