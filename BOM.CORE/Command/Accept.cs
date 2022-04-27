using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text; 
namespace BOM.CORE 
{  
    public class AcceptAlert : ICommand
    {
        int WaitTime = 0;
        ChromeDriver driver;
        public AcceptAlert(string pause)
        {
            this.WaitTime = Convert.ToInt32(pause ?? "0"); 
        } 
        public void Execute(ISessionContext ctx)
        {
            driver = ctx.SessionDriver.Driver;
            GetAlert(); 
        }
        private void GetAlert() {
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
            alert?.Accept(); 
        }
    } 
}
