using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOM.CORE 
{ 
    public class OpenTab : ICommand
    {
        string url = ""; 
        public OpenTab(string Url )
        {
            this.url = Url;
        }
        public void Execute(ISessionContext ctx)
        {
            var driver = ctx.SessionDriver.Driver;
            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            ctx.SessionDriver.GetUrl(this.url).Pause(20);
        }
    }  
}
