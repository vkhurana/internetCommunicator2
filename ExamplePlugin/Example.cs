﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSDK;

namespace EchoPlugin
{
    [StatelessChatPluginAttribute("echo's commands back on the same interface")]
    class EchoPlugin : IStatelessChatInterface
    {
        public string GetCommandResult(string cmdArgs)
        {
            return "hello world!";
        }
        public string GetCommandTrigger()
        {
            return "example";
        }
    }
}
