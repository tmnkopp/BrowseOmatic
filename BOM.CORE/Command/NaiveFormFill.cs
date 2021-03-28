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
        public NaiveFormFill( )
        { 
        }
        public void Execute(ISessionContext ctx)
        {
            Random _random = new Random();
            string rnd = string.Format("{0}:{1}"
                , DateTime.Now.TimeOfDay.Hours.ToString()
                , DateTime.Now.TimeOfDay.Minutes.ToString()) ;
            IList<IWebElement> inputs;
            var dvr = ctx.SessionDriver.Driver;
            string[] elements = new string[] { "*[type='text']", "textarea", "*[type='password']"};
            foreach (string el in elements)
            {
                inputs = dvr.FindElements(By.CssSelector(el));
                foreach (IWebElement input in inputs)
                {
                    ctx.SessionDriver.Pause(0); 
                    try
                    {
                        if (el == "textarea") {
                            input.Clear();
                            input.SendKeys(input.GetAttribute("id"));
                        }
                        if (el.Contains("password"))
                        {
                            input.Clear();
                            input.SendKeys(input.GetAttribute("type"));
                        }

                        var NaiveInputDefaults = ctx.SessionDriver.config.GetSection("NaiveInputDefaults");
                        if (NaiveInputDefaults != null)
                        {
                            foreach (var item in NaiveInputDefaults.GetChildren())
                            {
                                var target = $"{input.GetAttribute("name")} {input.GetAttribute("id")}";
                                if (Regex.Match(target, item.Key, RegexOptions.IgnoreCase).Success) {
                                    input.Clear();
                                    input.SendKeys(item.Value); 
                                }  
                            }
                        }

                        if (input.GetAttribute("value") == "") 
                            input.SendKeys("0");  
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"NaiveFormFill: {ex.Message}");
                    } 
                }
            }
            var selements = dvr.FindElements(By.TagName("select")); 
            foreach (var el in selements)
            {
                ctx.SessionDriver.Pause(0);
                SelectElement sections = new SelectElement(el);
                if (el != null)
                {
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
            foreach (string el in new string[] { "input[type='radio'], input[type='checkbox']" })
            {
                inputs = dvr.FindElements(By.CssSelector(el));
                foreach (IWebElement input in inputs)
                {
                    ctx.SessionDriver.Pause(0);
                    try
                    {
                        var val = input.GetAttribute("value");
                        if (val != "")
                            input.Click();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"NaiveFormFill: {ex.Message}");
                    }
                }
            } 
        
            inputs = dvr.FindElements(By.CssSelector("input[type='file']"));
            foreach (IWebElement input in inputs)
            {
                ctx.SessionDriver.Pause(0);
                try
                { 
                    input.SendKeys(@"C:\Users\Tim\Documents\upload.txt");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NaiveFormFill: {ex.Message}");
                }
            }

            inputs = dvr.FindElements(By.CssSelector("button[type='submit']"));
            foreach (IWebElement input in inputs)
            {
                ctx.SessionDriver.Pause(0);
                try
                {
                    //input.Click(); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NaiveFormFill: {ex.Message}");
                }
            }

        }
    }
}
