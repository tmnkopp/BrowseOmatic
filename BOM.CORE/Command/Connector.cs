using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{
    
    public class Connector 
    {
      
        private readonly IBScriptParser scriptParser;
        private string connstring;
        public Connector(IBScriptParser scriptParser, string connstring)
        {
            this.scriptParser = scriptParser;
            this.connstring = connstring;
        }
        public void Connect(ISessionDriver Driver)
        { 
            foreach (var spr in scriptParser.Parse(Driver.connstr))
            { 
                if (spr.QualifiedCommand == "GetUrl") Driver.GetUrl($"{spr.Arguments[0]}");
                if (spr.QualifiedCommand == "SendKeys") Driver.SendKeys(spr.Arguments[0], spr.Arguments[1]);
                if (spr.QualifiedCommand == "Click") Driver.Click(spr.Arguments[0]); 
            }
        }
    }
}
