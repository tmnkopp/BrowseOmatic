using BOM.CORE;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{ 
    public class UnittestCommand : ICommand
    {
        string key = "";
        string val = "";
        public UnittestCommand(string key, string val)
        {
            this.key = key;
            this.val = val;
        }
        public void Execute(ISessionContext ctx)
        {
            var c = ctx; 
        }
    }
}
