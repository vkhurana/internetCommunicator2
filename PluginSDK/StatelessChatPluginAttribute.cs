using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginSDK
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StatelessChatPluginAttribute : Attribute
    {
        public StatelessChatPluginAttribute(string description)
        {
            m_description = description;
        }

        private string m_description;

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }
    }
}
