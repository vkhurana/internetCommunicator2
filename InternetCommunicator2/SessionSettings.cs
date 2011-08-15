using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InterComm.Helpers;

namespace InterComm
{
    public static class SessionSettings
    {
        static SessionSettings()
        {
            Logger.LogInformation("loading settings...");
        }

        public static string[] builtincommands = new string[] {
                                                "testme"
                                                };

        public static string logfile = "InterComm.log";
        public static string pluginsfolder = "Plugins";
        public static string pluginsextensions = "dll";
        public static string usersfile = "ConfigFiles\\users.ini";
        public static string usersfile_temp = "ConfigFiles\\~tmp~users.ini";
        public static string connectserver = "talk.google.com";

        public static bool remoteserver
        {
            get
            {
                return File.Exists("deploy");
            }
        }

        // this logic is here so i dont have to make seperate binaries for my dev machine an depl machine
        public static string username
        {
            get
            {
                if (remoteserver)
                {
                    if (File.Exists("raptor"))
                        return "deployment_user1@gmail.com";
                    else
                        return "deployment_user2@gmail.com";
                }
                else
                {
                    return "local_user@gmail.com";
                }
            }
        }
        public static string password
        {
            get
            {
                if (remoteserver)
                {
                    if (File.Exists("raptor"))
                        return "deployment_user1_password";
                    else
                        return "deployment_user2_password";
                }
                else
                {
                    return "local_user_password";
                }
            }
        }
        public static string langaugesfile = "ConfigFiles\\languages.ini";

        public static string googleapikey = "<your google api key>";
        public static string googlesitereferrer = "some url";

        public static string defaultlanguage = "en";
        public static string defaultwelcomemessage = "hello";

        public static double connectionretryinterval = 30 * 1000;
        public static int connectionretryattempts = 10;

        public static string helpcommand = "help";
    }
}
