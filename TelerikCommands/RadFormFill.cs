using System;
using System.Collections.Generic;
using System.Linq;
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
        public override string ToString()
        {
            return $"RadFormFill: [{this.container}]";
        }
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
                    throw ex;
                }
                idlist.RemoveAt(0);
            }
        }
        public void Execute(ISessionContext ctx)
        {
            Random _random = new Random();
            string rnd = DateTime.Now.Day.ToString() + "-" + _random.Next(255).ToString();
            var dvr = ctx.SessionDriver;
            dvr.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(.25);
            
            InputIterator(dvr, " input[id$='rcbMultiSelect_Input']", (inputid) => { 
                IWebElement input = dvr.Pause(150).Driver.FindElement(By.CssSelector($"#{inputid}"));
                input.Click();
                dvr.Pause(150); 
                IList<IWebElement> options = dvr.Driver.FindElements(By.CssSelector($".rcbSlide .RadComboBoxDropDown li"));  
                foreach (IWebElement element in options)
                {
                    if (element.Displayed && element.Enabled) {
                        element.Click();  
                        break;
                    }  
                }
                dvr.Pause(150).Click("form[name='aspnetForm']");
            });

            dvr.Click(this.container);  

            InputIterator(dvr, " input[type='radio']", (inputid) => {
                IWebElement input = dvr.Pause(150).Driver.FindElement(By.CssSelector($"#{inputid}"));
                var pattern = ctx.configuration.GetSection("InputDefaults:RadFormFill:radio")?.Value ?? ".*"; 
                if (Regex.IsMatch(input.GetAttribute("value"), $"{pattern}")) input.Click(); 
            }); 
            InputIterator(dvr, " .RadDropDownList", (inputid) => {
                dvr.Pause(150).Driver.FindElement(By.CssSelector($"#{inputid}")).Click();
                dvr.Pause(250).Click("ul[class*='rddlList'] li:nth-child(2)").Pause(50);
            });
            // InputIterator(dvr, " .RadComboBox", (inputid) => {
            //     dvr.Pause(250).Driver.FindElement(By.CssSelector($"#{inputid}")).Click();
            //     dvr.Pause(250).Click("ul[class*='rcbList'] li:nth-child(2)").Pause(150);
            // });
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
                foreach (var input in inputs) {
                    var NaiveInputDefaults = ctx.SessionDriver.config.GetSection("NaiveInputDefaults");
                    if (NaiveInputDefaults != null)
                    {
                        foreach (var item in NaiveInputDefaults.GetChildren())
                        {
                            string rn = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 1).Select(s => s[new Random().Next(s.Length)]).ToArray());
                            string val = item.Value.Replace("\\w", rn); 
                            var target = $"{input?.GetAttribute("name")} {input?.GetAttribute("id")}";
                            if (Regex.Match(target, item.Key, RegexOptions.IgnoreCase).Success)
                                input?.SendKeys(val);
                        }
                    }
                    if (input?.GetAttribute("value") == "") input?.SendKeys("0");
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RadFormFill input[type='text']: {ex.Message}\n");
            } 

        }
    }
}
