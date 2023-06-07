using AutoItX3Lib;
using BOM;
using BOM.CORE;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnitTests
{
    [TestClass]
    public class Net
    {
        #region Fields
        private ILogger<Net> logger; 
        #endregion
        public Net()
        {
            var mock = new Mock<ILogger<Net>>();
            ILogger<Net> logger = mock.Object;
        } 
        List<BTask> tasks = new List<BTask>();
        [TestMethod]
        public void ebil_connects()
        {
            var ctx = Session.Context("ebil");
            var dvr = ctx.SessionDriver; //|.*Prepopulation.*  .*BOD.*Section.*[1-3].*|.*CSHELP-2899
            ctx.SessionDriver.Create();
            CommandProcessor processor = new CommandProcessor(ctx, logger);
            processor.Process(ctx.ContextConfig.conntask);
  
        }

        [TestMethod]
        public void CL_Subs()
        { 
            BTask task = new BTask("subl_search", "bom");  
            task.TaskSteps.Add(new TaskStep("Url", new string[] { "https://dayman.cyber-balance.com/jira/browse/CS-8450" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "opsbar-operations_more" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "log-work" }));
            task.TaskSteps.Add(new TaskStep("Key", new string[] { "input[id='log-work-time-logged']", "15m" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "input[id='log-work-submit']" }));
            task.TaskSteps.Add(new TaskStep("Pause", new string[] { "900" }));
            tasks.Add(task);
      
            //CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            //processor.Process(task);

            Utils.WriteTasks(tasks );
        }      
        [TestMethod]
        public void ebil_writes()
        {
            AutoItX3 autoit = new AutoItX3();
            var ctx = Session.Context("bomdriver");
            var dvr = ctx.SessionDriver;
            ctx.SessionDriver.Create(); 
            ctx.SessionDriver.Driver.Navigate().GoToUrl("https://rtime.raytheon.com/?t");  
            autoit.WinWait("Select a certificate", "", 5);
            autoit.ControlClick("Cancel", "Cancel", "Cancel");
           
            Thread.Sleep(3000);
             
            //  BTask task = new BTask("rtime", "rtime");
            //  task.TaskSteps.Add(new TaskStep("Url", new string[] { "https://secure.ebillity.com/firm4.0/TimeExpense/WeeklyTimeSheet2.aspx" }));
            //  tasks.Add(task);
            //  processor.Process(task);

            //task.TaskSteps.Add(new TaskStep("Script", new string[] { "document.getElementsByClassName('btn_green')[0].click();" }));  
             
        }
        [TestMethod]
        public void Logger_Resolves()
        {
            string raw = File.ReadAllText(@"d:\logs\log202201191240.txt");
            MatchCollection matches = Regex.Matches(raw, @"\[WRN\].*(\{.+invalid element state.+\})");
            raw = $"[{ string.Join(",", (from System.Text.RegularExpressions.Match m in matches select m.Groups[1].Value).ToList()) }]";

            string xpath = "//*[@id='ctl00_ContentPlaceHolder1_CBButtPanel1_btnEdit']";
            dynamic json = JsonConvert.DeserializeObject(raw);
            dynamic query = (from j in ((JArray)json)
                             where j["xpath"].ToString() == xpath
                             select j).FirstOrDefault();
            var item = query.method.Value;
        }
       
        [TestMethod]
        public void bomdriver_Resolves()
        {  
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver.Driver; 
            ctx.SessionDriver.Create();
            WebDriverWait wait = new WebDriverWait(dvr, TimeSpan.FromSeconds(1));
             
            CommandProcessor processor = new CommandProcessor(ctx, logger);
            processor.Process(ctx.ContextConfig.conntask);

            var mtx = new List<object[]>();
            mtx.Add(new object[] { "https://dayman.cyber-balance.com/jira/secure/Dashboard.jspa?selectPageId=12340" });
            mtx.Add(new object[] { "//table[@class='issue-table']//td[@class='issuekey']/a" });
            mtx.Add(new object[] { "//a[@id='opsbar-operations_more']//span[contains(., 'More')]" });
            mtx.Add(new object[] { "//span[contains(., 'Log')]" });
            mtx.Add(new object[] { "//input[contains(@id, 'log-work-time-logged')]", "15m" });
            mtx.Add(new object[] { "//input[contains(@id, 'log-work-submit')]" });
  
            foreach (object[] obs in mtx)
            {
                string xpath = (string)obs[0];
                if (xpath.StartsWith("http"))
                {
                    dvr.Navigate().GoToUrl(xpath);
                    continue;
                }
                object[] args = (obs.Count() > 0) ? obs.Skip(1).ToArray() : null; 
                var elm = (from e in wait.Until(drv => drv.FindElements(By.XPath($"{xpath}"))) 
                           select e).FirstOrDefault();
                try
                {
                    if (args.Count() == 0)
                    {
                        elm.Click();
                    }
                    else
                    {
                        elm.Clear();
                        elm.SendKeys(args[0].ToString());
                    }
                } 
                catch (Exception)
                {
                    throw;
                }
                if (AlertPresent(dvr))
                {
                    dvr.SwitchTo().Alert().Accept();
                }
            }
        }
        public static bool AlertPresent(ChromeDriver d)
        {
            try
            {
                d.SwitchTo().Alert();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    } 
}