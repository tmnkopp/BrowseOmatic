using System;
using System.Collections.Generic;
using System.Text;
using BOM.CORE;
using OpenQA.Selenium;

namespace BOM
{
    public class ToTab : ICommand
    {
        private string tab = "";
        public ToTab(string Tab)
        {
            this.tab = Tab;
        }
        public void Execute(ISessionContext ctx)
        { 
            IList<IWebElement> elements  = ctx.SessionDriver.Driver.FindElements(By.CssSelector("*[id*='_Surveys'] li"));
            foreach (IWebElement element in elements) {  
                if (element.Text.ToUpper().Contains($"{tab.ToUpper()}"))  {
                    element.Click(); 
                    break;
                }
            }
            System.Threading.Thread.Sleep(400);
        }
    }
}
