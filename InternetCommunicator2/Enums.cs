using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterComm
{
    /// <summary>
    /// internal enum, used to maintain the user states of each User object
    /// essential to determine who is allowed and who is not
    /// </summary>
    public enum UserState
    {
        Invalid,
        Removed,
        Allowed,
        Authenticated,
        Timedout,
        LoggedOut
    }

    /// <summary>
    /// maintains the state of the connection with the talk server
    /// used by each Protocol Implementation
    /// </summary>
    public enum ConnectionState
    {
        Initializing,
        Connecting,
        AuthenticationFailed,
        Connected,
        Disconnected,
        ForcedDisconnect
    }

    /// <summary>
    /// used to manage the friend requests that are received
    /// the allowed users are read from the users.ini configfile
    /// </summary>
    public enum FriendRequest
    {
        Add,
        Delete,
        Approve,
        Deny
    }

    /// <summary>
    /// abstraction over all the possible states the bot can be in
    /// these four are for pretty much all the possible protocols
    /// </summary>
    public enum StatusInfo
    {
        Available,
        Unavailable,
        Away,
        Invisible
    }

    public enum UserAuthLevel
    {
        Unauthorized,
        Regular,
        Admin,
    }

}
