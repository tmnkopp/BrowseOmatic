using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BOM.CORE 
{
    public class NaiveFormFill : ICommand
    {
        private string container = "";
        public NaiveFormFill(string Container )
        {
            this.container = Container;
        } 
        public void Execute(ISessionContext ctx)
        {
            Random _random = new Random();
            string rnd = string.Format("{0}:{1}"
                , DateTime.Now.TimeOfDay.Hours.ToString()
                , DateTime.Now.TimeOfDay.Minutes.ToString()) ;
            IList<IWebElement> inputs;
            var dvr = ctx.SessionDriver.Driver;
            ctx.SessionDriver.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(ctx.SessionDriver.Timeout);
            string[] elements;
             
            inputs = dvr.FindElements(By.CssSelector($"{this.container} select"));
            List<string> ids = new List<string>();
            foreach (IWebElement input in inputs) ids.Add(input.GetAttribute("id") ?? "");
            foreach (string id in ids) 
            { 
                IWebElement el = dvr.FindElement(By.CssSelector($"#{id}")); 
                if (el != null)
                {
                    SelectElement sections = new SelectElement(el);
                    int _from = 2;
                    while (_from > 0)
                    {
                        _from--;
                        try
                        {
                            int index = _random.Next(_from, sections.Options.Count - 1); 
                            sections.SelectByIndex(index);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"sections.SelectByIndex index out of range {e.Message}");
                        } 
                    } 
                }
            }  
            foreach (string el in new string[] { $"{this.container} input[type='radio']", $"{this.container}  input[type='checkbox']" })
            { 
                inputs = dvr.FindElements(By.CssSelector(el));
                ids = new List<string>();
                try
                {
                    foreach (IWebElement input in inputs) ids.Add(input.GetAttribute("id") ?? "");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"NFF: radio, checkbox:  {e.Message}"); 
                }
                
                foreach (string id in ids)
                {  
                    try
                    {
                        dvr.FindElement(By.CssSelector($"#{id}"))?.Click(); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"NaiveFormFill: {ex.Message} {el} {id}");
                    }
                }
            } 
         
            elements = new string[] { $"{this.container} *[type='text']", $"{this.container} textarea", $"{this.container} *[type='password']" };
            foreach (string el in elements)
            {
                inputs = dvr.FindElements(By.CssSelector(el));
                ids = new List<string>();
                foreach (IWebElement input in inputs) ids.Add(input.GetAttribute("id") ?? "");
                foreach (string id in ids)
                { 
                    try
                    {
                        IWebElement input = dvr.FindElement(By.CssSelector($"#{id}"));
                        input?.Clear();
                        if (el == "textarea") input?.SendKeys(id); 
                        if (el.Contains("password")) input?.SendKeys(id);
                    
                        var NaiveInputDefaults = ctx.SessionDriver.config.GetSection("NaiveInputDefaults");
                        if (NaiveInputDefaults != null)
                        {
                            foreach (var item in NaiveInputDefaults.GetChildren())
                            {
                                var target = $"{input?.GetAttribute("name")} {input?.GetAttribute("id")}";
                                if (Regex.Match(target, item.Key, RegexOptions.IgnoreCase).Success) 
                                    input?.SendKeys(item.Value); 
                            }
                        }  
                        if (input?.GetAttribute("value") == "") input?.SendKeys("0");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"NaiveFormFill: {ex.Message} {el} {id}");
                    }
                }
            } 
        }
    }
}
