using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{  
    public class AlertAccept : ICommand
    {
        int WaitTime = 0; 
        public AlertAccept(string pause)
        {
            this.WaitTime = Convert.ToInt32(pause ?? "0"); 
        } 
        public void Execute(ISessionContext ctx)
        {
            var driver = ctx.SessionDriver.Driver;
            IAlert alert = null; 
            WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(WaitTime)); 
            try
            {
                alert = wait.Until(d =>
                {
                    try
                    { 
                        return driver.SwitchTo().Alert();
                    }
                    catch (NoAlertPresentException)
                    { 
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException) { alert = null; } 
        }
    } 
}
