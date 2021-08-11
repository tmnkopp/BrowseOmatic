using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{
    public class Key : ICommand
    {
        string element = "";
        string content = "";
        public Key(string Element, string Content)
        {
            this.element = Element;
            this.content = Content;
        }
        public override string ToString()
        {
            return $"Key: ['{this.element}', '{this.content}']";
        }
        public void Execute(ISessionContext ctx)
        {
            ctx.SessionDriver.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(ctx.SessionDriver.Timeout);
            ctx.SessionDriver.SendKeys(this.element, this.content);
        }
    }
}
