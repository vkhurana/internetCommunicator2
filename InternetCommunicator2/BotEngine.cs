using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using InterComm.Helpers;
using System.Windows;
using System.Globalization;

namespace InterComm
{
    /*
    public delegate void MessageHandlerCallback(string from, string message);
    public delegate void PresenceInformationCallback(string from, FriendRequest action);
    */
    class BotEngine
    {
        private ConnectionState iXMPPConnecttionState;
        private frmMain MainUI;

        ProtocolImpl iXMPPConnection;
        UserManager iUserManager;
        CommandHandler iCommandHandler;

        LanguageSelector iLanguageSelector;
        
        /// <summary>
        /// constructor - make the basic objects here
        /// </summary>
        public BotEngine(frmMain mainForm)
        {
            iXMPPConnection = new ProtocolImpl();
            iXMPPConnecttionState = ConnectionState.Initializing;
            MainUI = mainForm;
        }

        /// <summary>
        /// Initialization calls, setup all the handlers and the callbacks here
        /// also establish the connection
        /// </summary>
        public void Initialize()
        {
            /*MessageHandlerCallback messageHandler = new MessageHandlerCallback(this.MessageReceived);
            PresenceInformationCallback presenceHandler = new PresenceInformationCallback(this.PresenceHandler);*/

            //initialize the plugin handler
            //Logger.LogInformation("initializing connection process...");
            iXMPPConnection.Initialize(this/*messageHandler, presenceHandler*/);
            //make it connect
            //Logger.LogInformation("connecting...");
            iXMPPConnection.Connect();
            //check the state
            if (iXMPPConnection.State != ConnectionState.Connected && iXMPPConnection.State != ConnectionState.Connecting)
            {
                Logger.LogInformation("Unable to connect - " + iXMPPConnection.GetDetailedConnectionState());
                throw new Exception("error connecting to talk service");
            }
            iUserManager = new UserManager();
            iLanguageSelector = new LanguageSelector();
            iCommandHandler = new CommandHandler(this);
            //initialize the plugins and other message handlers here
        }

        /// <summary>
        /// gets the internal user data struct that is maintained for each user
        /// </summary>
        /// <param name="username">email address of the chap to return</param>
        /// <returns>User data struct maintained by the user manager</returns>
        private User GetUserFromUserManagerByUsername(string username)
        {
            return iUserManager.GetUser(username);
        }

        public void UpdateConnectionState(ConnectionState state)
        {
            iXMPPConnecttionState = state;
            MainUI.UpdateStatusInfo(state.ToString());
        }

        /// <summary>
        /// handles new user requests
        /// </summary>
        /// <param name="user">username of the presence informaion</param>
        /// <param name="action">type of presence information received</param>
        public void PresenceHandler(string user, FriendRequest action)
        {
            switch (action)
            {
                case FriendRequest.Add:
                    {
                        //a user just added the bot
                        //check using user manager and approve or deny the request
                        User newUser = GetUserFromUserManagerByUsername(user);
                        if (newUser.State == UserState.Allowed)
                        {
                            Logger.LogInformation("accepting user request from new user: " + newUser.UserName + " (" + newUser.NickName + ")");
                            iXMPPConnection.HandleFriendRequest(user, FriendRequest.Approve);
                        }
                        else
                        {
                            Logger.LogInformation("DENYING user request from new user: " + user);
                            iXMPPConnection.HandleFriendRequest(user, FriendRequest.Deny);
                        }
                        break;
                    }
                case FriendRequest.Delete:
                    {
                        //user just deleted the bot
                        //todo(2) should i ban him, or set him as unauthorized? or just ignore the deletion.
                        //send him an email asking him why? :-P hehe...
                        break;
                    }
                default:
                    {
                        Logger.LogInformation("invalid presence information received " + action.ToString());
                        throw new NotImplementedException("invalid presence request received");
                    }
            }
        }

        /// <summary>
        /// this is the core of the application. all the message handling is initiated here. :-O
        /// </summary>
        /// <param name="from"></param>
        /// <param name="clientMessage"></param>
        public void MessageReceived(string from, string clientMessage)
        {
            //MainUI.AddToListBox(from + "|" + clientMessage);
            //i know who the message is from... lets get his details
            User fromUser = GetUserFromUserManagerByUsername(from);
            if (!fromUser.Allowed)
            {
                iXMPPConnection.SendMessage(fromUser.UserName, "You do not have access");
                return;
            }
            switch (fromUser.State)
            {
                case UserState.Allowed:
                    {
                        //initial state - give the user his welcome message here
                        //authentication (if i decide to add it) will also initiate here
                        LanguageSelector.LanguageInfo defaultLanguage = iLanguageSelector.GetLanguageInfo(SessionSettings.defaultlanguage);
                        LanguageSelector.LanguageInfo randomLanguage = iLanguageSelector.GetRandomLanguage();
                        string greeting = TranslateAPI.Translator.TranslateText(
                                                                    SessionSettings.defaultwelcomemessage,
                                                                    new CultureInfo(defaultLanguage.langcode , false),
                                                                    new CultureInfo(randomLanguage.langcode, false)
                                                                    );
                        greeting = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(greeting); //capitalize the first char of the greeting
                        iXMPPConnection.SendMessage(fromUser.UserName, greeting + " " + fromUser.NickName + "!" + " (" + randomLanguage.fullname + ")");
                        //the user has been greeted, open him up to commands!
                        fromUser.Authenticated();
                        iXMPPConnection.SetStatus("Last Authenticated: " + DateTime.Now.ToString(), StatusInfo.Available);
                        break;
                    }
                case UserState.Authenticated:
                    {
                        //user authenticated. lets get busy here. this is the core of the command handling
                        string[] commandAndArgs = clientMessage.Split(new char[] { ' ' });
                        string command = commandAndArgs[0];
                        string args = string.Join(" ", commandAndArgs, 1, commandAndArgs.Length - 1);
                        iCommandHandler.HandleCommand(fromUser, command, args);
                        break;
                    }
                default:
                    {
                        Logger.LogInformation("BUG! invalid user state detected");
                        break;
                    }
            }
        }

        public bool AddUser(string username) { return AddUser(username, null); }
        public bool AddUser(string username, string nickname)
        {
            //todo(1) make sure you dont add a user that already exists

            EmailAddress add = new EmailAddress(username);
            if (!add.Valid) throw new Exception("Invalid email address provided!");
            string nick = nickname;
            if (nick == null)
            {
                nick = add.Username;
            }

            bool addUserResult = false;
            addUserResult = iUserManager.AddUser(username, nick, UserAuthLevel.Regular); //this is the default auth level for all users added remotely

            //user has been added to the file, now send out the request
            iXMPPConnection.HandleFriendRequest(username, FriendRequest.Add);

            return addUserResult;
        }

        public bool RemoveUser(string username)
        {
            //todo(1) make sure you dont try to remove a user that doesnt exist!

            EmailAddress add = new EmailAddress(username);
            if (!add.Valid) throw new Exception("Invalid email address provided!");
            bool removeUserResult = false;
            removeUserResult = iUserManager.RemoveUser(username);

            iXMPPConnection.HandleFriendRequest(username, FriendRequest.Delete);

            return removeUserResult;
        }

        public void SendMessage(User toUser, string message)
        {
            iXMPPConnection.SendMessage(toUser, message);
        }

            //now i know that fromuser is acceptabe, lets see what commad he has sent.

            //steps involved :
            //1. check the user
            //  - is the user allowed?
            //  - is the user authenticated?
            //  - is the user in the correct state... has the user logged out/timed out?
            //  - 
            //2. handle the message
            //MessageBox.Show(clientMessage);
    }
}
