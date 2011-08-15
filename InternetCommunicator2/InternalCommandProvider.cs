using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterComm.Helpers;
using InterComm.InternalCommands;

namespace InterComm
{
    class InternalCommandProvider
    {
        List<CommandBase> _internalCommands = new List<CommandBase>();

        public InternalCommandProvider(BotEngine engine)
        {
            //constructor - add all the commads here
            _internalCommands.Add(new RemoteUserManagement(engine));
            _internalCommands.Add(new WhoAmI(engine));
            _internalCommands.Add(new Tweet(engine));
            _internalCommands.Add(new Service(engine));
            _internalCommands.Add(new SystemController(engine));
            _internalCommands.Add(new RemoteExec(engine));

        }

        public List<CommandBase> InternalCommands
        {
            get
            {
                return _internalCommands;
            }
        }
    }
}
