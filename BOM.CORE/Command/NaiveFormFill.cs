using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BOM.CORE 
{
    public class NaiveFormFill : ICommand
    {
     
        public NaiveFormFill()
        { 
        }
        public void Execute(ISessionContext ctx)
        {
            Random _random = new Random();
            string rnd = string.Format("-{0}:{1}"
                , DateTime.Now.TimeOfDay.Hours.ToString()
                , DateTime.Now.TimeOfDay.Minutes.ToString()) ;
            IList<IWebElement> inputs;
            var dvr = ctx.SessionDriver.Driver;
            string[] elements = new string[] { "*[type='text']", "textarea" };
            foreach (string el in elements)
            {
                inputs = dvr.FindElements(By.CssSelector(el));
                foreach (IWebElement input in inputs)
                {
                    try
                    {
                        var val = input.GetAttribute("value");
                        if (val == "")
                            input.SendKeys("0");
                        if (Regex.Match(val, "\\w*").Success && val.Length < 50)
                            input.SendKeys($" {rnd}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"NaiveFormFill: {ex.Message}");
                    }

                }
            }
        }
    }
}
