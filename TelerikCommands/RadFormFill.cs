using System;
using System.Collections.Generic;
using System.Text;
using BOM.CORE;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace TelerikCommands
{
    public class RadFormFill : ICommand
    {
        private string container = "";
        public RadFormFill(string Container)
        {
            this.container = Container;
        }
        public void Execute(ISessionContext ctx)
        {
            Random _random = new Random();
            string rnd = DateTime.Now.Day.ToString() + "-" + _random.Next(255).ToString();
            var dvr = ctx.SessionDriver;
             
            IList<IWebElement> inputs;
            try
            {
                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container} input[id*='MultiSelect_Input']"));
                dvr.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
                List<string> ids = new List<string>();
                foreach (IWebElement input in inputs) ids.Add(input.GetAttribute("id") ?? "");
                foreach (string id in ids)
                {
                    IWebElement el = dvr.Driver.FindElement(By.CssSelector($"#{id}"));
                    el.Click();
                    dvr.Pause(450);
                    IList<IWebElement> options = dvr.Driver.FindElements(By.CssSelector($".rcbSlide .rcbList li"));
                    if (options.Count > 1)
                    { 
                        int index = _random.Next(1, options.Count - 1);
                        options[2].Click(); 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"sections.SelectByIndex index out of range {ex.Message}");
            } 
            try
            { 
  
                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container}  .RadDropDownList"));
                foreach (var input in inputs)
                {
                    input.Click();
                    dvr.Pause(250).Click("ul[class*='rddlList'] li:nth-child(2)").Click("body").Pause(50);
                } 
                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container}  .RadDropDownList"));
                foreach (var input in inputs) { 
                    input.Click();
                    dvr.Pause(250).Click("ul[class*='rddlList'] li:nth-child(2)").Click("body").Pause(50);
                }
                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container}  .RadComboBox"));
                foreach (var input in inputs)
                {
                    input.Click();
                    dvr.Pause(450).Click("ul[class*='rcbList'] li:nth-child(2)").Click("body").Pause(150);
                } 
                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container}  input[id*='date']"));
                foreach (var input in inputs)
                    if (input.GetAttribute("value") == "") input.SendKeys($"{DateTime.Now.ToShortDateString()}");

                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container}  input[type='text']"));
                foreach (var input in inputs)
                    if (input.GetAttribute("value") == "") input.SendKeys($"0");

                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container}  .RadGrid textarea"));
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
