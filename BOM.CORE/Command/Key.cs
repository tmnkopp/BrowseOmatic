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
        public void Execute(ISessionContext ctx)
        {
            ctx.SessionDriver.Pause(0);
            ctx.SessionDriver.SendKeys(this.element, this.content);
        }
    }
}
