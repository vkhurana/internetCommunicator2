using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterComm.InternalCommands
{
    class CommandDescriptor
    {
        public CommandDescriptor(User from, string args)
        {
            this.fromUser = from;
            this.args = args;
        }
        public User fromUser { get; set; }
        public string args { get; set; }
    }
}
