using BOM.CORE;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace BOM.CS
{
    public class CsDriver : SessionDriver, ISessionDriver
    {
        private readonly IBScriptParser scriptParser;
        private readonly IConfiguration configuration;
        public string connstr { get; set; }
        public CsDriver(
            IConfiguration configuration,
            IBScriptParser scriptParser,
            string ConnectionString) : base(configuration, scriptParser, ConnectionString)
        {
            connstr = ConnectionString;
            this.scriptParser = scriptParser;
        }
        public CsDriver GetSection(string Element, int index)
        {
            SelectElement sections = new SelectElement(Select(Element));
            if (sections != null)
            {
                try
                {
                    sections.SelectByIndex(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"sections.SelectByIndex index out of range {e.Message}");
                }
            }
            return this;
        }
        public CsDriver ToTab(string tab)
        {
            IList<IWebElement> elements;
            elements = Driver.FindElements(By.XPath("//div[@id='ctl00_ContentPlaceHolder1_radTS_Surveys']//li"));
            foreach (IWebElement element in elements)
                if (element.Text.ToUpper().Contains($"{tab.ToUpper()}"))
                {
                    element.Click();
                    break;
                }
            return this;
        }
        public void FormFill() { 
            
        }
    }
}
