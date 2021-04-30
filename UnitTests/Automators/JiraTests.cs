using BOM.CORE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnitTests
{
    [TestClass]
    public class JiraTests
    {
        [TestMethod]
        public void JiraIssue_Starter()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver;
            var urlProvider = new UrlProvider(".issue-table tr .summary a[href*='browse/CS-81']", ".*BOD.*Script.*");
            urlProvider.Execute(ctx);
            dvr.Pause(2000);
            dvr.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            foreach (KeyValuePair<string,string> kvp in urlProvider.Hrefs)
            {
                dvr.Pause(900).GetUrl(kvp.Key);
                dvr.Pause(900).Click("opsbar-operations_more");   
                dvr.Pause(900).Click("log-work");
                dvr.Pause(900).SendKeys("input[id='log-work-time-logged']", "15m");
                dvr.Pause(200).Click("input[id='log-work-submit']");
                //dvr.Pause(1000).Click("a[title*='Start Progress']");    // Interconnections Database Script
                //dvr.Pause(1200).Click("a[title*='Resolve']");    
                //dvr.Pause(100).Click("input[id*='issue-workflow-transition-submit']"); 
                //dvr.Pause(100).Click("a[title*='Ready To Test']");
                //dvr.Pause(100).Click("input[id*='issue-workflow-transition-submit']");
            }
            dvr.Dispose();
        }
    }
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
                if ( Regex.IsMatch($"{href}{txt}", MatchPattern))
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
