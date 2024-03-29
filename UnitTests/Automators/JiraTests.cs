using BOM;
using BOM.CORE;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnitTests
{ 
    [TestClass]
    public class JiraTests
    {
        CommandProcessor processor;
        private string selector;
        private string matchPattern;
        List<BTask> tasks = new List<BTask>();
        public JiraTests()
        {
            selector = "//*[text()[contains(.,'SA:')] and contains(@class, 'issue-link') and not (contains(.,'D~atabase'))]"; 
            matchPattern = "Section";
        }
        [TestMethod]
        public void JiraTime_Taker()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver; 
            ctx.SessionDriver.Create(); 
            CommandProcessor processor = new CommandProcessor(ctx, new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(ctx.ContextConfig.conntask);

            var urlProvider = new UrlProvider(selector);
            urlProvider.Execute(ctx);
            BTask task = new BTask("taketime", "jira");
            foreach (KeyValuePair<string, string> kvp in urlProvider.Items)
            { 
                task.TaskSteps.Add(new TaskStep("Url", new string[] { kvp.Key }));
                task.TaskSteps.Add(new TaskStep("SetWait", new string[] { "1" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "opsbar-operations_more" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "log-work" }));
                task.TaskSteps.Add(new TaskStep("Key", new string[] { "input[id='log-work-time-logged']", "15m" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "input[id='log-work-submit']" }));
                task.TaskSteps.Add(new TaskStep("Pause", new string[] { "2500" }));  
            }
            task.TaskSteps.Add(new TaskStep("Url", new string[] { "https://dayman.cyber-balance.com/jira/" }));
            processor.Process(task);
            Utils.WriteTask(task);
            dvr.Dispose(); 
        }
        [TestMethod]
        public void JiraIssue_Opener()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver;
            ctx.SessionDriver.Create();

            CommandProcessor processor = new CommandProcessor(ctx, new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(ctx.ContextConfig.conntask);

            var urlProvider = new UrlProvider(selector);
            urlProvider.Execute(ctx);
            BTask task = new BTask("start_tickets", "jira");
            foreach (KeyValuePair<string, string> kvp in urlProvider.Items)
            {
                task.TaskSteps.Add(new TaskStep("Url", new string[] { kvp.Key }));
                task.TaskSteps.Add(new TaskStep("Pause", new string[] { "1200" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "a[title*='Start Progress']" })); 
                task.TaskSteps.Add(new TaskStep("Pause", new string[] { "900" }));
                tasks.Add(task);
            }
            processor.Process(task);
            Utils.WriteTask(task);
            //dvr.Dispose();
        }
        [TestMethod]
        public void JiraIssue_Resolver()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver;
            ctx.SessionDriver.Create();

            CommandProcessor processor = new CommandProcessor(ctx, new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(ctx.ContextConfig.conntask);

            var urlProvider = new UrlProvider(selector);
            urlProvider.Execute(ctx);
            BTask task = new BTask("resolve_tickets", "jira");
            foreach (KeyValuePair<string,string> kvp in urlProvider.Items)
            { 
                task.TaskSteps.Add(new TaskStep("Url", new string[] { kvp.Key }));
                task.TaskSteps.Add(new TaskStep("Pause", new string[] { "1500" }));  
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "a[title*='Resolve']" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "input[id*='issue-workflow-transition-submit']" }));
                task.TaskSteps.Add(new TaskStep("Pause", new string[] { "1500" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "a[title*='Ready To Test']" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "input[id*='issue-workflow-transition-submit']" }));
                task.TaskSteps.Add(new TaskStep("Pause", new string[] { "1500" }));
                tasks.Add(task);
            }
            processor.Process(task);
            // Utils.WriteTask(task);
            dvr.Dispose();
        }
        [TestMethod]
        public void JiraIssue_Scraper()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver; //|.*Prepopulation.*  .*BOD.*Section.*[1-3].*|.*CSHELP-2899
            ctx.SessionDriver.Create( );
            BTask task = new BTask("scrape_issue", "jira");
            task.TaskSteps.Add(new TaskStep("Url", new string[] { "https://dayman.cyber-balance.com/jira/si/jira.issueviews:issue-html/CS-8621/CS-8621.html" }));
            task.TaskSteps.Add(new TaskStep("ClickByContent", new string[] { "Validate and Approve" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "header-operations" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "allCsvFields" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "csv-export-dialog-export-button" }));
            tasks.Add(task);
            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);
            //Utils.WriteTasks(tasks);
            //dvr.Dispose();
        }
    } 
}
