using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{  
    public class SwitchTo : ICommand
    {
        int tab = 0; 
        public SwitchTo(int Tab)
        {
            this.tab = Tab; 
        } 
        public void Execute(ISessionContext ctx)
        {
            var handles = ctx.SessionDriver.Driver.WindowHandles;
            if (this.tab >= handles.Count || this.tab < 0) 
                this.tab = handles.Count - 1; 
            ctx.SessionDriver.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(ctx.SessionDriver.Timeout); 
            ctx.SessionDriver.Driver.SwitchTo().Window(handles[this.tab]); 
        }
    } 
}
