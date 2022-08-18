using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BOM.CORE
{
    public class UrlProvider 
    {
        #region CTOR  
        private string XPath = "";
        public UrlProvider(string XPath)
        {
            this.XPath = XPath; 
        } 
        #endregion

        #region PROPS
        private Dictionary<string, string> items = new Dictionary<string, string>();
        public Dictionary<string, string> Items
        {
            get { return items; }
        }
        #endregion

        #region METHODS

        public void Execute(ISessionContext ctx)
        {
            IList<IWebElement> inputs =  ctx.SessionDriver.Driver.FindElements(By.XPath($"{XPath}"));
            foreach (var input in inputs)
            {
                var txt = input.Text;
                var href = input.GetAttribute("href");
                items.Add(href, txt);
            }
        }
        #endregion
    }
}
