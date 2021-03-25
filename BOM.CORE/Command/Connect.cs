using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{
    
    public class Connect : ICommand
    {
      
        private readonly IBScriptParser scriptParser;
        private string context;
        public Connect(IBScriptParser scriptParser, string context)
        {
            this.scriptParser = scriptParser;
            this.context = context;
        }
        public void Execute(ISessionContext ctx)
        {
            var Driver = ctx.SessionDriver.Driver;
            foreach (var spr in scriptParser.Parse(ctx.SessionDriver.connstr))
            { 
                if (spr.QualifiedCommand == "GetUrl") Driver.Navigate().GoToUrl($"{spr.Arguments[0]}");
                if (spr.QualifiedCommand == "SendKeys") ctx.SessionDriver.SendKeys(spr.Arguments[0], spr.Arguments[1]);
                if (spr.QualifiedCommand == "Click") ctx.SessionDriver.Click(spr.Arguments[0]); 
            }
        }
    }
}
