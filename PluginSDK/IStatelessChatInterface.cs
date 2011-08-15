using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginSDK
{
    public interface IStatelessChatInterface
    {
        string GetCommandResult(string cmdArgs);
        string GetCommandTrigger();
    }
}
