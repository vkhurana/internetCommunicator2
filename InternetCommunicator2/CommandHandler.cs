using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSDK;
using InterComm.InternalCommands;
using InterComm.Helpers;

namespace InterComm
{
    class CommandHandler
    {
        PluginProvider iLoadedPlugins;
        InternalCommandProvider iInternalCommands;
        BotEngine iEngine;

        public CommandHandler(BotEngine botEngine)
        {
            iInternalCommands = new InternalCommandProvider(botEngine);
            //todo(2) while loading the plugins, make sure a plugin with an internal command
            //as the command trigger is not loaded, potentially, pass in iInternalCommands to
            //PluginProvider... is that the best way? right now i just have a list. no no no...
            iLoadedPlugins = new PluginProvider();

            iEngine = botEngine;
        }

        private string GenerateCommandHelpString(User from)
        {
            StringBuilder helpCommand = new StringBuilder();
            helpCommand.Append("\nCommands you can use...");
            string commandFormat = "\n{0}";
            foreach (CommandBase internalcommand in iInternalCommands.InternalCommands)
            {
                if (internalcommand.MinAuthLevel <= from.AuthLevel) //need to make sure only commands with the correct auth level are returned!
                {
                    //helpCommand.Append("\n\"" + internalcommand.CommandTrigger + "\"");
                    helpCommand.Append(string.Format(commandFormat, internalcommand.CommandTrigger));
                }
            }
            foreach (PluginWrapper plugin in iLoadedPlugins.Plugins) //no auth level stuff here, all commands work!
            {
                //helpCommand.Append("\n\"" + plugin.CommandTrigger + "\"");
                helpCommand.Append(string.Format(commandFormat, plugin.CommandTrigger));
            }
            return helpCommand.ToString();
        }

        public void HandleCommand(User from, string command, string args)
        {
            if (!from.Authorized)
            {
                Logger.LogInformation("BUG! command from unauthorized user... caught at at a level i wouldnt like");
                return;
            }

            //help command!
            if (string.Compare(SessionSettings.helpcommand, command, true) == 0)
            {
                string returnString;
                try
                {
                    returnString = GenerateCommandHelpString(from);
                }
                catch (Exception ex)
                {
                    returnString = "Error getting list of commands!\n(" + ex.ToString() + ")";
                }
                iEngine.SendMessage(from, returnString);
                return;
            }

            //internal commands get the highest priority
            foreach (CommandBase internalcommand in iInternalCommands.InternalCommands)
            {
                if (string.Compare(internalcommand.CommandTrigger, command, true) == 0
                    && internalcommand.MinAuthLevel <= from.AuthLevel)
                {
                    CommandDescriptor cmd = new CommandDescriptor(from, args);
                    internalcommand.ProcessCommand(cmd);
                    return;
                }
            }

            //these are stateless. single command sent, single reply received plugins
            foreach (PluginWrapper plugin in iLoadedPlugins.Plugins)
            {
                if (string.Compare(plugin.CommandTrigger, command, true)==0) //case insensitive
                {
                    string result = plugin.GetCommandResult(args);
                    iEngine.SendMessage(from, result);
                    return;
                }
            }
            iEngine.SendMessage(from, "Unrecognized Command! try: \"help\"");
            return;
        }
    }
}
