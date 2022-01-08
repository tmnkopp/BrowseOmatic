using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{  
    public class Click : ICommand
    {
        string element = ""; 
        public Click(string Element)
        {
            this.element = Element; 
        } 
        public void Execute(ISessionContext ctx)
        {
            foreach (string ele in element.Split(","))
            {
                ctx.SessionDriver.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(ctx.SessionDriver.Timeout);
                ctx.SessionDriver.Click(ele);
            } 
        }
    } 
}
