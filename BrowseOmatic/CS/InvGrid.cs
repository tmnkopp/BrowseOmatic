using System;
using System.Collections.Generic;
using System.Text;
using BOM.CORE;
using OpenQA.Selenium;

namespace BOM
{

    public class InvGrid : ICommand
    {
        private string tab = "";
        public InvGrid(string Tab)
        {
            this.tab = Tab;
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
                System.Threading.Thread.Sleep(200);
                IList<IWebElement> txts = Driver.FindElements(By.CssSelector(".rgEditRow *[type='text']"));
                foreach (var txt in txts)
                {
                    txt.Clear();
                    txt.SendKeys("0");
                }
                SessionContext.SessionDriver.Click("UpdateButton").Click("[onclick*='submit']").Pause(350);
                Driver.SwitchTo().Alert().Accept();
                System.Threading.Thread.Sleep(200);
            }   
        }
    }
}
