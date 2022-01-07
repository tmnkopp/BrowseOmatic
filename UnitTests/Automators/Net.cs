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
    public class Net
    {
        List<BTask> tasks = new List<BTask>();
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
        public void JiraIssue_Action()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver; //|.*Prepopulation.*  .*BOD.*Section.*[1-3].*|.*CSHELP-2899
            ctx.SessionDriver.Create();
            var urlProvider = new UrlProvider(".issue-table tr .summary a[href*='browse/CS-8']", ".*SAOP.*Section.*");
            urlProvider.Execute(ctx); 
            foreach (KeyValuePair<string,string> kvp in urlProvider.Items)
            {
                dvr.GetUrl(kvp.Key); 
                ctx.SessionDriver.Timeout = 2; 
                 dvr.Click("a[title*='Resolve']")
                 .Pause(950).Click("input[id*='issue-workflow-transition-submit']")
                 .Pause(950).Click("a[title*='Ready To Test']")
                 .Pause(950).Click("input[id*='issue-workflow-transition-submit']"); 
            }  
            dvr.Dispose();
        }
        [TestMethod]
        public void ebil_writes()
        {
            BTask task = new BTask("ebil", "ebil");
            task.TaskSteps.Add(new TaskStep("Url", new string[] { "https://secure.ebillity.com/firm4.0/TimeExpense/WeeklyTimeSheet2.aspx" }));
            task.TaskSteps.Add(new TaskStep("Pause", new string[] { "900" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "btnCopyTimeSheet" }));
            task.TaskSteps.Add(new TaskStep("Pause", new string[] { "900" }));
            //task.TaskSteps.Add(new TaskStep("Script", new string[] { "document.getElementsByClassName('btn_green')[0].click();" }));  
            tasks.Add(task);  
            Utils.WriteTasks(tasks);
        } 
    } 
}
