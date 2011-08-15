using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using agsXMPP;
using InterComm.Helpers;
using agsXMPP.protocol.client;
using System.Timers;

namespace InterComm
{
    /// <summary>
    /// i should make this class an interface. just so you can plug any protocol behind it. todo(2).
    /// </summary>
    class ProtocolImpl
    {
        private BotEngine iEngine;
        //private variable to monitor the connection state
        private ConnectionState _state = ConnectionState.Initializing;
        private ConnectionState iState
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                iEngine.UpdateConnectionState(_state);
            }
        }
        //the protocol handler. thanks agsxmpp
        private XmppClientConnection iConnection = new XmppClientConnection();
        //callbaqck delegate to handle the callback functions
        //MessageHandlerCallback messageHandlerCB;
        //callback delegate to handle presence callback
        //PresenceInformationCallback presenceHandlerCB;
        //timer used for connection retries
        Timer retryTimer;
        int retryAtttempts;

        /// <summary>
        /// property to determine the current connection state. internal state.
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return iState;
            }
        }
        /// <summary>
        /// gets the connection state of the protocol handler. string value of state maintained by agsxmpp.
        /// </summary>
        /// <returns></returns>
        public string GetDetailedConnectionState() { return iConnection.XmppConnectionState.ToString(); }

        /// <summary>
        /// Call this function to initialize the protocol implementation. It sets up the handlers.
        /// </summary>
        public void Initialize(BotEngine engine/*MessageHandlerCallback messageHandler, PresenceInformationCallback presenceHandler*/)
        {
            iEngine = engine;
            //engine must be initialized before changing state!
            iState = ConnectionState.Initializing;
            iConnection.OnLogin += new ObjectHandler(onLogin);
            iConnection.OnAuthError += new XmppElementHandler(onAuthError);
            iConnection.OnClose += new ObjectHandler(onClose);
            iConnection.OnError += new ErrorHandler(onError);
            iConnection.OnXmppConnectionStateChanged += new XmppConnectionStateHandler(onXmppConnectionStateChanged);
            iConnection.OnMessage += new agsXMPP.protocol.client.MessageHandler(onMessage);
            iConnection.OnPresence += new PresenceHandler(onPresence);


            //set the callback function in the bot engine to react to the messages being received
            //messageHandlerCB = messageHandler;
            //set the callback function for presence information
            //presenceHandlerCB = presenceHandler;

            retryTimer = new Timer();
            retryTimer.AutoReset = false; //i want a one shot timer, ill decide what to do the next time after that
            retryTimer.Elapsed += new ElapsedEventHandler(retryTimer_Tick);
            retryTimer.Enabled = false;
            retryAtttempts = SessionSettings.connectionretryattempts;
        }

        private void retryTimer_Tick(object sender, ElapsedEventArgs e)
        {
            //check state to see if its connected...
            if (iState != ConnectionState.Connected)
            {
                if (retryAtttempts > 0)
                {
                    //still have some retry events left
                    //disconnect and connect again...
                    retryAtttempts--;
                    Reconnect();
                }
                else
                {
                    //ran out of retry events..
                    Logger.LogInformation("\n\nERROR CONNECTING AFTER ALL RETRY ATTEMPTS!\n\n");
                    retryTimer.Enabled = false;
                    retryTimer.Stop();
                }
            }
            else
            {
                //all good :)
                //successful connect!
                retryTimer.Stop();
                retryTimer.Enabled = false;
                //incase it gets disconnected again, ill need to use these values
                retryTimer.Interval = SessionSettings.connectionretryinterval;
                retryAtttempts = SessionSettings.connectionretryattempts;
            }
        }

        /// <summary>
        /// call this function to take the bot offline
        /// </summary>
        public void Disconnect()
        {
            Logger.LogInformation("disconnecting...");
            iState = ConnectionState.ForcedDisconnect;
            iConnection.Close();
        }

        /// <summary>
        /// call this function if you want to reconnect the connection
        /// </summary>
        public void Reconnect()
        {
            Logger.LogInformation("reconnecting...");
            Disconnect();
            Connect();
        }

        /// <summary>
        /// Call this function to initiate the connection process
        /// </summary>
        public void Connect()
        {
            if (iState == ConnectionState.Connected ||
                iState == ConnectionState.Connecting)
            {
                Logger.LogInformation("BUG! connect called when in connected/connecting state");
                return;
            }
            iState = ConnectionState.Connecting;
            Jid user = new Jid(SessionSettings.username);
            iConnection.Username = user.User;
            iConnection.Server = user.Server;
            iConnection.ConnectServer = SessionSettings.connectserver;
            iConnection.Password = SessionSettings.password;
            //iConnection.AutoResolveConnectServer = true;
            iConnection.Open();
            //start the timer, give it 30 seconds to connect...
            Logger.LogInformation("retry timer started..."); //hopefully we wont need it!
            retryTimer.Enabled = true;
            retryTimer.Interval = SessionSettings.connectionretryinterval;
            retryTimer.Start();
        }

        /// <summary>
        /// This function is called once the connection is established.
        /// </summary>
        /// <param name="sender">?</param>
        private void onLogin(object sender)
        {
            if (iConnection.Authenticated == true)
            {
                Logger.LogInformation("connection success!");
                iState = ConnectionState.Connected;
            }
            else
            {
                Logger.LogInformation("connection failure!");
            }
        }

        /// <summary>
        /// called when there is an authentcation failure, probably means that G has disabled
        /// my account. >:-(
        /// </summary>
        void onAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            Logger.LogInformation("authentication failure");
            iState = ConnectionState.AuthenticationFailed;
        }

        /// <summary>
        /// called when the connection is closed.
        /// </summary>
        void onClose(object sender)
        {
            Logger.LogInformation("connection closed");
            if (iState != ConnectionState.ForcedDisconnect)
            {
                iState = ConnectionState.Disconnected;
                Connect();
            }
        }

        /// <summary>
        /// called whenever there is a protocol error.
        /// </summary>
        void onError(object sender, Exception ex)
        {
            Logger.LogInformation("error in connection");
            Logger.LogException(ex);
        }

        /// <summary>
        /// manages the connection state machine
        /// </summary>
        void onXmppConnectionStateChanged(object sender, XmppConnectionState state)
        {
            /*
            Disconnected, //session disconnected
            Connecting, //socket connecting
            Connected, //socket connected
            Authenticating,
            Authenticated,
            Binding,
            Binded,
            StartSession,
            StartCompression,
            Compressed,
            SessionStarted, //??
            Securing, //normal to ssl
            Registering, //new acount
            Registered //success
             * to
            Initializing,
            Connecting,
            AuthenticationFailed,
            Connected,
            Disconnected
            */
            Logger.LogInformation("XMPP Con state changed to:" + state.ToString());
            if (state == XmppConnectionState.Disconnected) iState = ConnectionState.Disconnected;
            if (state == XmppConnectionState.SessionStarted) iState = ConnectionState.Connected;
        }

        /// <summary>
        /// messsage received.. since this is just the protocol handler, we just send the message along...
        /// </summary>
        void onMessage(object sender, Message msg)
        {
            Logger.LogInformation(msg.Type + " : " + msg.From + " - " + msg.Body);
            //callback to Main Handler?
            iEngine.MessageReceived(msg.From.Bare, msg.Body);
        }

        /// <summary>
        /// called when the server send presence information about a user
        /// </summary>
        void onPresence(object sender, Presence pres)
        {
            Logger.LogInformation("presence information: " + pres.From + ":" + pres.Type.ToString());
            switch (pres.Type)
            {
                case PresenceType.subscribe:
                    {
                        //new friend add request (someone just added the bot)
                        iEngine.PresenceHandler(pres.From.Bare, FriendRequest.Add);
                        break;
                    }
                case PresenceType.unsubscribe:
                    {
                        //new delete request (someone just deleted the bot from their friend list)
                        iEngine.PresenceHandler(pres.From.Bare, FriendRequest.Delete);
                        break;
                    }
                case PresenceType.available:
                case PresenceType.invisible:
                case PresenceType.unavailable:
                    {
                        //do i need to do something about this?
                        break;
                    }
            }
            //pres.Type = agsXMPP.protocol.client.PresenceType.subscribe;
            ////send this:
            //// <presence to='contact@example.org' type='subscribe'/>
            //Presence pres = new Presence();
            //pres.Type = PresenceType.subscribed;
            //pres.To = to;

            //m_connection.Send(pres);
        }

        /// <summary>
        /// this function is used to send messages to users
        /// </summary>
        public void SendMessage(User to, string message)
        {
            SendMessage(to.UserName, message);
        }
        public void SendMessage(string to, string message)
        {
            Message sendmsg = new Message();
            sendmsg.Type = MessageType.chat;
            sendmsg.To = new Jid(to);
            sendmsg.Body = message;
            iConnection.Send(sendmsg);
        }

        /// <summary>
        /// handle presence information request from the bot engine,possible actions : 
        /// accept - accept a friend request
        /// reject - reject a friend request
        /// add - add a user to the bots roster
        /// remove - remove a user from the bots roster
        /// </summary>
        /// <param name="user"></param>
        /// <param name="action"></param>
        public void HandleFriendRequest(string user, FriendRequest action)
        {
            Presence pres = new Presence();
            pres.To = new Jid(user);

            switch (action)
            {
                case FriendRequest.Approve:
                    {
                        pres.Type = PresenceType.subscribed;
                        break;
                    }
                case FriendRequest.Deny:
                    {
                        pres.Type = PresenceType.unsubscribed;
                        break;
                    }
                case FriendRequest.Add:
                    {
                        pres.Type = PresenceType.subscribe;
                        break;
                    }
                case FriendRequest.Delete:
                    {
                        pres.Type = PresenceType.unsubscribe;
                        break;
                    }
            }
            iConnection.Send(pres);
        }



        /// <summary>
        /// sets the status message and icon of the bot
        /// </summary>
        /// <param name="statusMessage">teh message that is to be displayed</param>
        /// <param name="icon">icon to be displayed</param>
        public void SetStatus(string statusMessage, StatusInfo icon)
        {
            switch (icon)
            {
                case StatusInfo.Available:
                    {
                        iConnection.Show = ShowType.chat;
                        break;
                    }
                case StatusInfo.Unavailable:
                    {
                        iConnection.Show = ShowType.dnd;
                        break;
                    }
                case StatusInfo.Away:
                    {
                        iConnection.Show = ShowType.away;
                        break;
                    }
                default:
                    {
                        iConnection.Show = ShowType.NONE;
                        break;
                    }
            }
            iConnection.Status = statusMessage;
            Logger.LogInformation("setting status = " + iConnection.Show.ToString() + " - " + iConnection.Status.ToString());
            iConnection.SendMyPresence();
        }
    }
}
