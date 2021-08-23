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
        private string MatchPattern = "";
        private string CssSelector = "";
        public UrlProvider(string CssSelector, string MatchPattern)
        {
            this.CssSelector = CssSelector;
            this.MatchPattern = MatchPattern;
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
            IList<IWebElement> inputs =  ctx.SessionDriver.Driver.FindElements(By.CssSelector($"{CssSelector}"));
            foreach (var input in inputs)
            {
                var txt = input.Text;
                var href = input.GetAttribute("href");
                if (Regex.IsMatch($"{href}{txt}", MatchPattern))
                {
                    if (!items.ContainsKey(href))
                    {
                        items.Add(href, txt);
                    }
                }
            }
        }
        #endregion
    }
}
