using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{
    public class NaiveFormFill : ICommand
    {
        string container = "";
        public NaiveFormFill(string Container)
        {
            this.container = Container;
        }
        public void Execute(ISessionContext ctx)
        {
            Random _random = new Random();
            IList<IWebElement> inputs;
            var dvr = ctx.SessionDriver.Driver;
            string[] elements = new string[] { "*[type='text']", "textarea" };
            foreach (string el in elements)
            {
                inputs = dvr.FindElements(By.CssSelector(el));
                foreach (IWebElement input in inputs)
                {
                    if (input.GetAttribute("value") == "")
                        input.SendKeys("0");
                }
            }
        }
    }
}
