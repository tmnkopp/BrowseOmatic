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
            this.url = Url.Trim();
        }
        public override string ToString()
        {
            return $"OpenTab: ['{this.url}']";
        }
        public void Execute(ISessionContext ctx)
        { 
            this.url = this.url.Replace("~", ctx.ContextConfig.root ?? "");
            var driver = ctx.SessionDriver.Driver;
            if (string.IsNullOrEmpty(this.url)) 
                this.url = driver.Url;
       
            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            ctx.SessionDriver.GetUrl(this.url).Pause(20); 
        }
    }  
}
