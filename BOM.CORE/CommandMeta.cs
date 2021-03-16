using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandMeta : Attribute
    {
        private string context;
        public string Context
        {
            get { return context; }
            set { context = value; }
        }
        public CommandMeta(string Context)
        {
            context = Context;
        }
    }
}
