using BOM.CORE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace UnitTests
{
    [TestClass]
    public class JiraTests
    {
        StringBuilder sb = new StringBuilder();
        [TestMethod]
        public void URL_Logger()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver;  
            string[] urls = new string[] { "CS-8450" }; // , "CS-8412"
            foreach (var item in urls)
            { 
                dvr.Pause(500).GetUrl($"https://dayman.cyber-balance.com/jira/browse/{item}");//
                ctx.SessionDriver.Timeout = 2;
                dvr.Click("opsbar-operations_more"); 
                dvr.Click("log-work"); 
                dvr.SendKeys("input[id='log-work-time-logged']", "15m"); 
                dvr.Click("input[id='log-work-submit']"); 
            } 
            dvr.Dispose();
        }
        [TestMethod]
        public void JiraIssue_Starter()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver; //|.*Prepopulation.*  .*BOD.*Section.*[1-3].*|.*CSHELP-2899
            var urlProvider = new UrlProvider(".issue-table tr .summary a[href*='browse/CS-8']", ".*SAOP.*Section.*");
            urlProvider.Execute(ctx); 
            foreach (KeyValuePair<string,string> kvp in urlProvider.Hrefs)
            {
                dvr.GetUrl(kvp.Key);
                sb.AppendLine($"    - Click: ['{kvp.Key}']");
                ctx.SessionDriver.Timeout = 2;
                //dvr.Pause(550).Click("a[title*='Start Progress']").Pause(550);
                dvr.Click("opsbar-operations_more"); 
                dvr.Click("log-work"); 
                dvr.Pause(900).SendKeys("input[id='log-work-time-logged']", "15m"); 
                dvr.Pause(1200).Click("input[id='log-work-submit']").Pause(1200);
                // dvr.Click("a[title*='Resolve']")
                // .Click("input[id*='issue-workflow-transition-submit']")
                // .Click("a[title*='Ready To Test']")
                // .Click("input[id*='issue-workflow-transition-submit']");

                //string log = File.ReadAllText(@"C:\Users\Tim\Documents\LOGS\log.txt");
                //File.WriteAllText(@"C:\Users\Tim\Documents\LOGS\log.txt", $"{ctx.SessionDriver.Driver.Title}\n{log}" + ""  , Encoding.Unicode);

            }
            var s = sb.ToString();
            File.WriteAllText($"c:\\bom\\unittest\\jira.yaml", s, Encoding.Unicode);
            dvr.Dispose();
        }
    }
  
}
