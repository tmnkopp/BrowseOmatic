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
                dvr.Pause(250); 
                IList<IWebElement> items = dvr.Driver.FindElements(By.CssSelector($".rcbSlide .RadComboBoxDropDown li .rcbCheckBox"));
                foreach (IWebElement item in items)  
                {  
                    var prop = (item.GetProperty("checked") ?? "").ToLower();
                    try
                    {
                        if (item.Displayed && item.Enabled && prop != "true")
                            item.Click();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{item.ToString()} {inputid}: {ex.Message}\n");
                        throw ex;
                    } 
                } 
                dvr.Pause(150).Click("form[name='aspnetForm']"); // TODO: Move to Config File
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
                            string val = item.Value; 
                            val = FormatWildcard(val); 
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
        private string FormatWildcard(string val) {
            StringBuilder sb = new StringBuilder();
            string[] vals;
            if (val.Contains("\\d"))
            {
                vals = val.Split(new string[] { "\\d" }, StringSplitOptions.None);
                for (int i = 1; i < vals.Length; i++)
                    sb.Append($"{GetRandNum()}{vals[i]}");
                val = $"{vals[0]}{sb.ToString()}";
            }
             
            sb.Clear();
            if (val.Contains("\\w"))
            {
                vals = val.Split(new string[] { "\\w" }, StringSplitOptions.None);
                for (int i = 1; i < vals.Length; i++)
                    sb.Append($"{GetRand()}{vals[i]}");
                val = $"{vals[0]}{sb.ToString()}";
            } 
            return val;
        }
        private string GetRand() {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 1).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
        private string GetRandNum()
        {
            return new string(Enumerable.Repeat("0123456789", 1).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
}
