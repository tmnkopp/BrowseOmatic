using System;
using System.Collections.Generic;
using System.Text;
using BOM.CORE;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TelerikCommands
{

    public class InvGrid : ICommand
    { 
        private string container;
        public InvGrid( string Container)
        {
            this.container = Container; 
        }
        public void Execute(ISessionContext SessionContext)
        { 
            var Driver = SessionContext.SessionDriver.Driver;
            
            while (true)
            {
                IList<IWebElement> elements = Driver.FindElements(By.CssSelector("*[id*='EditButton']"));
                if (elements.Count < 1) 
                    break; 
                elements[0].Click();
                System.Threading.Thread.Sleep(100);
                IList<IWebElement> txts = Driver.FindElements(By.CssSelector(".rgEditRow *[type='text']"));
                foreach (var txt in txts)
                {
                    txt.Clear();
                    txt.SendKeys("0");
                }
                ((IJavaScriptExecutor)Driver).ExecuteScript("window.scrollTo({left: 0});");
                System.Threading.Thread.Sleep(250); 

                var id = Driver.FindElements(By.CssSelector(".rgEditRow input[id*='_UpdateButton']"))[0].GetAttribute("id");
                ((IJavaScriptExecutor)Driver).ExecuteScript($"document.getElementById('{id}').click();");

                System.Threading.Thread.Sleep(250); 
                Driver.FindElements(By.CssSelector("[onclick*='submit']"))[0].Click(); 

                Driver.SwitchTo().Alert().Accept();
                System.Threading.Thread.Sleep(250);
            }   
        }
    }
}
