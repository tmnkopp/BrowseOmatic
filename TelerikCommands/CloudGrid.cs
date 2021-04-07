using System;
using System.Collections.Generic;
using System.Text;
using BOM.CORE;
using OpenQA.Selenium;

namespace TelerikCommands
{
    public class CloudGrid : ICommand
    {
        private string container = "";
        public CloudGrid(string Container)
        {
            this.container = Container;
        }
        public void Execute(ISessionContext SessionContext)
        {
            Random _random = new Random(); 
            string rnd = DateTime.Now.Day.ToString() + " " + _random.Next(255).ToString();

            var dvr = SessionContext.SessionDriver; 
            dvr.Click("AddNewRecordButton"); 
            IList<IWebElement> inputs;

            inputs = dvr.Driver.FindElements(By.CssSelector(".RadDropDownList"));
            foreach (var input in inputs)
            {
                input.Click();
                dvr.Pause(200).Click("ul[class*='rddlList'] li:nth-child(2)");
            }

            inputs = dvr.Driver.FindElements(By.CssSelector("input[id*='date']"));
            foreach (var input in inputs)
                if (input.GetAttribute("value") == "") input.SendKeys($"{DateTime.Now.ToShortDateString()}");

            inputs = dvr.Driver.FindElements(By.CssSelector("input[type='text']"));
            foreach (var input in inputs)
                if (input.GetAttribute("value") == "") input.SendKeys($"{rnd}");

            try
            {
                dvr.Click("PerformInsertButton");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            
        }
    }
}
