using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterComm
{
    public class User
    {
        /*
         * 
         * 
         * THIS CLASS HAS A COPY CONSTRUCTOR! MAKE SURE YOU EDIT IT IF YOU ADD OR REMOVE PROPERTIES
         * 
         * 
         */ 
        private string _username;
        private UserState _state;
        private string _nickname;
        private UserAuthLevel _authLevel;

        public User(User u)
        {
            _username = u.UserName;
            _state = u.State;
            _nickname = u.NickName;
            _authLevel = u.AuthLevel;
        }

        public User(string username, string nickname, UserState state, UserAuthLevel auth)
        {
            _username = username;
            _state = state;
            _nickname = nickname;
            _authLevel = auth;
        }

        public string UserName
        {
            get
            {
                return _username;
            }
        }

        public UserState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public string NickName
        {
            get
            {
                return _nickname;
            }
        }

        public bool Allowed
        {
            get
            {
                return (
                        _state == UserState.Allowed ||
                        _state == UserState.Authenticated
                        ) ? true : false;
            }
        }

        public bool Authorized
        {
            get
            {
                return _authLevel == UserAuthLevel.Unauthorized ? false : true;
            }
        }

        public void Authenticated()
        {
            State = UserState.Authenticated;
        }

        public UserAuthLevel AuthLevel
        {
            get
            {
                return _authLevel;
            }
            set
            {
                _authLevel = value;
            }
        }
    }
}
