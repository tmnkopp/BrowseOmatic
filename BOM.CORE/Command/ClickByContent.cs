using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BOM.CORE 
{  
    public class ClickByContent : ICommand
    {
        string ElementSelect = ""; 
        string Contents = "";
        bool UseRegex = false;
        public ClickByContent(string ElementSelect, string Contents, bool UseRegex)
        {
            this.ElementSelect = ElementSelect; 
            this.Contents = Contents; 
            this.UseRegex = UseRegex; 
        }
        public void Execute(ISessionContext ctx)
        { 
            ctx.SessionDriver.Pause(0);
            IList<IWebElement> elements = ctx.SessionDriver.Driver.FindElements(By.CssSelector($"{this.ElementSelect}"));
            foreach (IWebElement element in elements)
            {
                string txt = element.Text;
                if (!UseRegex)
                {
                    if (txt.ToUpper().Contains($"{Contents.ToUpper()}"))
                    {
                        element.Click(); break;
                    }
                }
                else 
                {
                    if (Regex.IsMatch(txt, $"{Contents}"))
                    {
                        element.Click(); break;
                    }
                }
  
            } 
        }
    } 
} 