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
        List<BTask> tasks = new List<BTask>();
        [TestMethod]
        public void JiraIssue_TimeTaker()
        { 
            BTask task = new BTask("taketime", "jira");  
            task.TaskSteps.Add(new TaskStep("Url", new string[] { "https://dayman.cyber-balance.com/jira/browse/CS-8450" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "opsbar-operations_more" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "log-work" }));
            task.TaskSteps.Add(new TaskStep("Key", new string[] { "input[id='log-work-time-logged']", "15m" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "input[id='log-work-submit']" }));
            task.TaskSteps.Add(new TaskStep("Pause", new string[] { "900" }));
            tasks.Add(task);
      
            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);

            WriteTasks(tasks );
        }       
        [TestMethod]
        public void JiraIssue_List()
        { 
            BTask task = new BTask("export_all", "jira"); 
            task.TaskSteps.Add(new TaskStep("Url", new string[] { "https://dayman.cyber-balance.com/jira/issues/?jql=project%20%3D%20CS%20AND%20assignee%20%3D%20currentUser()%20AND%20type%20%3DTask%20AND%20resolution%20%3D%20Unresolved" }));
            task.TaskSteps.Add(new TaskStep("SetWait", new string[] { "2" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "header-operations" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "allCsvFields" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "csv-export-dialog-export-button" })); 
            task.TaskSteps.Add(new TaskStep("Script", new string[] {  @" 
                document.title='TASKS';
                window.scrollTo(100, 1000); 
            " })); 
            tasks.Add(task); 
            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);

            WriteTasks(tasks);
        }
        [TestMethod]
        public void JiraCreate()
        {
            BTask task = new BTask("create", "jira");
            task.TaskSteps.Add(new TaskStep("SetWait", new string[] { "2" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "create_link" })); 
            task.TaskSteps.Add(new TaskStep("Script", new string[] { @" 
                document.title='TASKS';
                window.scrollTo(100, 1000); 
            " }));
            tasks.Add(task);
            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);

            WriteTasks(tasks);
        }

        [TestMethod]
        public void JiraTime_Taker()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver; 
            ctx.SessionDriver.Connect(ctx.configContext.conn);
            var urlProvider = new UrlProvider(".issue-table tr .summary a[href*='browse/CS-85']", ".*EINSTEIN.*");
            urlProvider.Execute(ctx);
            BTask task = new BTask("ein_taketime", "jira");
            foreach (KeyValuePair<string, string> kvp in urlProvider.Items)
            { 
                task.TaskSteps.Add(new TaskStep("Url", new string[] { kvp.Key }));
                task.TaskSteps.Add(new TaskStep("SetWait", new string[] { "2" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "opsbar-operations_more" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "log-work" }));
                task.TaskSteps.Add(new TaskStep("Key", new string[] { "input[id='log-work-time-logged']", "45m" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "input[id='log-work-submit']" }));
                task.TaskSteps.Add(new TaskStep("Pause", new string[] { "1500" })); 
            }
            tasks.Add(task);
            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);
            WriteTasks(tasks);
            dvr.Dispose();
        }
        [TestMethod]
        public void JiraIssue_Starter()
        {
            var ctx = Session.Context("jira");
            var dvr = ctx.SessionDriver; //|.*Prepopulation.*  .*BOD.*Section.*[1-3].*|.*CSHELP-2899
            var urlProvider = new UrlProvider(".issue-table tr .summary a[href*='browse/CS-8']", ".*SAOP.*Section.*");
            urlProvider.Execute(ctx); 
            foreach (KeyValuePair<string,string> kvp in urlProvider.Items)
            {
                dvr.GetUrl(kvp.Key); 
                ctx.SessionDriver.Timeout = 2;
                //dvr.Pause(550).Click("a[title*='Start Progress']").Pause(550); 
                // dvr.Click("a[title*='Resolve']")
                // .Click("input[id*='issue-workflow-transition-submit']")
                // .Click("a[title*='Ready To Test']")
                // .Click("input[id*='issue-workflow-transition-submit']"); 
            }  
            dvr.Dispose();
        }
        private void WriteTasks(List<BTask> tasks )
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            File.WriteAllText($"c:\\bom\\unittest\\{tasks[0].Context}_{tasks[0].Name}.yaml", serializer.Serialize(tasks), Encoding.Unicode);
            File.WriteAllText($"c:\\bom\\unittest\\{tasks[0].Context}_{tasks[0].Name}.ps1", $"bom run -t {tasks[0].Name} -k -p c:\\bom\\unittest\\{tasks[0].Context}_{tasks[0].Name}.yaml", Encoding.Unicode);
        }
    }
  
}
