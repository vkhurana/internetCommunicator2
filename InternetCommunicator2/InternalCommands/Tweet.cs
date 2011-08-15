using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Windows.Forms;

using InterComm.Helpers;

namespace InterComm.InternalCommands
{
    class Tweet : CommandBase
    {

        private const string TwitterXMLUrl = "http://twitter.com/statuses/update.json";
        private const string Token = "<get this value from twitter>";
        private const string TokenSecret = "<get this value from twtiter>";
        private const string ConsumerKey = "<get this value from twitter>";
        private const string ConsumerKeySecret = "<get this value from twitter>";
        private const string VerifyURL = "https://api.twitter.com/oauth/authorize";//"http://twitter.com/account/verify_credentials.xml";

        private oAuthTwitter oAuth = null;
        private bool _authResult = false;

        public Tweet(BotEngine engine)
            : base(engine)
        {
            oAuth = new oAuthTwitter();
            oAuth.Token = Token;
            oAuth.TokenSecret = TokenSecret;
            oAuth.ConsumerKey = ConsumerKey;
            oAuth.ConsumerSecret = ConsumerKeySecret;
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
                return "tweet";
            }
        }

        private string SendTwitterMessage(string message)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            string tweet = HttpUtility.UrlEncode(message);
            string returnString = "null";

            try
            {

                string xml = oAuth.oAuthWebRequest(oAuthTwitter.Method.POST, TwitterXMLUrl, "status=" + tweet);
                returnString = "Posted Successfully!";
            }
            catch (Exception ex)
            {
                Logger.LogInformation("error from twitter...");
                Logger.LogException(ex);
                returnString = "Error! (" + ex.Message + ")";
            }
            return returnString;
        }

        public override void ProcessCommand(CommandDescriptor cmd)
        {
            string response = "null";
            response = SendTwitterMessage(cmd.args);

            SendReply(cmd, response);
            return;
        }
    }
}
