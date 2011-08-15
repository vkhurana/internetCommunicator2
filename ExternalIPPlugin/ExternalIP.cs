using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSDK;
using System.Net;
using System.IO;

namespace ExternalIPPlugin
{
    [StatelessChatPluginAttribute("gets the external ip address of the machine")]
    class ExternalIP : IStatelessChatInterface
    {
        private string getWebData(string aWebPage)
        {
            string responseSTR = "Error occured!";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(aWebPage);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                responseSTR = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                responseSTR = "Error " + ex.Message;
            }
            return responseSTR;
        }
        public string GetCommandResult(string cmdArgs)
        {
            return getWebData("http://www.whatismyip.org/");
        }
        public string GetCommandTrigger()
        {
            return "ipaddress";
        }
    }
}
