using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BOM.CORE
{
    public class UrlProvider : ICommand
    {
        #region CTOR

        private string MatchPattern = "";
        private string Selector = "";
        public UrlProvider(string Container, string MatchPattern)
        {
            this.Selector = Container;
            this.MatchPattern = MatchPattern;
        }

        #endregion

        #region PROPS
        private Dictionary<string, string> hrefs = new Dictionary<string, string>();
        public Dictionary<string, string> Hrefs
        {
            get { return hrefs; }
        }
        #endregion

        #region METHODS

        public void Execute(ISessionContext ctx)
        {
            IList<IWebElement> inputs =
                ctx.SessionDriver.Driver.FindElements(By.CssSelector($"{Selector}"));
            foreach (var item in inputs)
            {
                var txt = item.Text;
                var href = item.GetAttribute("href");
                if (Regex.IsMatch($"{href}{txt}", MatchPattern))
                {
                    if (!hrefs.ContainsKey(href))
                    {
                        hrefs.Add(href, txt);
                    }
                }
            }
        }
        #endregion
    }
}
