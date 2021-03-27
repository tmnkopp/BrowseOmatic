using System;
using System.Collections.Generic;
using System.Text;
using BOM.CORE;
using OpenQA.Selenium;

namespace BOM
{
    public class RadFormFill : ICommand
    { 
        public RadFormFill()
        { 
        }
        public void Execute(ISessionContext ctx)
        {
            Random _random = new Random();
            string rnd = DateTime.Now.Day.ToString() + "-" + _random.Next(255).ToString();
            var dvr = ctx.SessionDriver;
         
            IList<IWebElement> inputs; 
            try
            {
                inputs = dvr.Driver.FindElements(By.CssSelector(".RadDropDownList"));
                foreach (var input in inputs) { 
                    input.Click();
                    dvr.Pause(250).Click("ul[class*='rddlList'] li:nth-child(2)");
                    dvr.Click("body").Pause(50);
                }
                inputs = dvr.Driver.FindElements(By.CssSelector(".RadComboBox"));
                foreach (var input in inputs)
                {
                    input.Click();
                    dvr.Pause(450).Click("ul[class*='rcbList'] li:nth-child(2)");
                    dvr.Click("body").Pause(150);
                } 
                inputs = dvr.Driver.FindElements(By.CssSelector("input[id*='date']"));
                foreach (var input in inputs)
                    if (input.GetAttribute("value") == "") input.SendKeys($"{DateTime.Now.ToShortDateString()}");

                inputs = dvr.Driver.FindElements(By.CssSelector("input[type='text']"));
                foreach (var input in inputs)
                    if (input.GetAttribute("value") == "") input.SendKeys($"1");

                inputs = dvr.Driver.FindElements(By.CssSelector(".RadGrid textarea"));
                foreach (var input in inputs)
                    if (input.GetAttribute("value") == "") input.SendKeys($"{rnd}");
                 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RadFormFill: {ex.Message}\n");
            }
        }
    }
}
