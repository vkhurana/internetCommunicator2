using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterComm
{
    class SecurityDescriptor
    {
        public UserAuthLevel GetUserPermissions(User user)
        {
            return user.AuthLevel;
        }
    }
}
