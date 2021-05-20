using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BOM.CORE;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TelerikCommands
{
    public class RadFormFill : ICommand
    {
        private string container = "";
        public RadFormFill(string Container)
        {
            this.container = Container;
        }
        private void InputIterator(ISessionDriver dvr, string Selector, Action<string> InputAction) { 
            List<string> idlist = new List<string>();
            IList<IWebElement> inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container} {Selector}")); 
            foreach (IWebElement input in inputs) idlist.Add(input.GetAttribute("id") ?? "");
            while (idlist.Count > 0)
            {
                try  
                {
                    InputAction(idlist[0]); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{Selector} {idlist[0]}: {ex.Message}\n");
                }
                idlist.RemoveAt(0);
            }
        }
        public void Execute(ISessionContext ctx)
        {
            Random _random = new Random();
            string rnd = DateTime.Now.Day.ToString() + "-" + _random.Next(255).ToString();
            var dvr = ctx.SessionDriver;
            dvr.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(.5);

            InputIterator(dvr, " input[id*='MultiSelect']", (inputid) => { 
                IWebElement input = dvr.Pause(250).Driver.FindElement(By.CssSelector($"#{inputid}"));
                input.Click();
                dvr.Pause(250);
                List<string> idlist = new List<string>();
                IList<IWebElement> options = dvr.Driver.FindElements(By.CssSelector($".rcbSlide .RadComboBoxDropDown li"));  
                foreach (IWebElement element in options)
                {
                    if (element.Displayed && element.Enabled) {
                        element.Click();
                        break;
                    }  
                } 
                dvr.Pause(250);
            }); 
            InputIterator(dvr, " input[type='radio']", (inputid) => {
                IWebElement input = dvr.Pause(150).Driver.FindElement(By.CssSelector($"#{inputid}"));
                var pattern = ctx.configuration.GetSection("InputDefaults:RadFormFill:radio")?.Value ?? ".*"; 
                if (Regex.IsMatch(input.GetAttribute("value"), $"{pattern}")) input.Click(); 
            }); 
            InputIterator(dvr, " .RadDropDownList", (inputid) => {
                dvr.Pause(150).Driver.FindElement(By.CssSelector($"#{inputid}")).Click();
                dvr.Pause(250).Click("ul[class*='rddlList'] li:nth-child(2)").Click("body").Pause(50);
            });
            InputIterator(dvr, " .RadComboBox", (inputid) => {
                dvr.Pause(250).Driver.FindElement(By.CssSelector($"#{inputid}")).Click();
                dvr.Pause(250).Click("ul[class*='rcbList'] li:nth-child(2)").Click("body").Pause(150);
            });
            InputIterator(dvr, " select", (inputid) => {
                IWebElement input = dvr.Pause(150).Driver.FindElement(By.CssSelector($"#{inputid}"));
                SelectElement sections = new SelectElement(input);
                sections.SelectByIndex(sections.Options.Count-1);  
            });
            InputIterator(dvr, "textarea", (inputid) => {
                IWebElement input = dvr.Pause(250).Driver.FindElement(By.CssSelector($"#{inputid}"));
                if (input.GetAttribute("value") == "") input.SendKeys($"{input.GetAttribute("id")}");
            });

            IList<IWebElement> inputs;
            List<string> idlist = new List<string>(); 
            try
            { 
                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container}  input[id*='date']"));
                foreach (var input in inputs)
                    if (input.GetAttribute("value") == "") input.SendKeys($"{DateTime.Now.ToShortDateString()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RadFormFill input[id*='date']: {ex.Message}\n");
            }
            try
            {
                inputs = dvr.Driver.FindElements(By.CssSelector($"{this.container}  input[type='text']"));
                foreach (var input in inputs)
                    if (input.GetAttribute("value") == "") input.SendKeys($"0");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RadFormFill input[type='text']: {ex.Message}\n");
            } 
        }
    }
}
