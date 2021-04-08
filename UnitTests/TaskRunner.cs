using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using BOM;
using BOM.CORE;
using Moq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Xml;
using System.Text.RegularExpressions;

namespace UnitTests
{

    [TestClass]
    public class TaskRunner
    {
 
        [TestMethod]
        public void TaskStep_Steps()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("jira");
            var collection = database.GetCollection<BsonDocument>("issues");

            var ctx = Utils.Context();
            var epiclink = "CS-8098";
            var epic = "2021 FISMA Annual IG Data Call";
            ctx.SessionDriver.GetUrl("https://dayman.cyber-balance.com/jira/browse/" + epiclink); 

            IList<IWebElement> inputs = ctx.SessionDriver.Driver.FindElements(By.CssSelector("table[id='ghx-issues-in-epic-table'] .ghx-minimal a"));
            List<string> items = new List<string>();
            foreach (var item in inputs) items.Add(item.Text);
            foreach (var item in items)
            { 
                ctx.SessionDriver.Pause(500).GetUrl($"https://dayman.cyber-balance.com/jira/si/jira.issueviews:issue-xml/{item}/{item}.xml"); 
                var src = ctx.SessionDriver.Driver.PageSource.ToString(); 
                src = src.Substring(src.IndexOf("<item>"), src.IndexOf("</item>") - src.IndexOf("<item>")) + "</item>";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(src);
                string title = doc.SelectSingleNode("//title")?.InnerText.Trim() ?? "";
                string section = Regex.Replace(title, "\\[.*\\] ", ""); 
                var post = new BsonDocument  {
                        {"issuekey" , item},
                        {"epiclink" , epiclink},
                        {"epic" , epic},
                        {"title" , title},
                        {"section" , section ?? ""},
                        {"link" , doc.SelectSingleNode("//link")?.InnerText.Trim() ?? ""},
                        {"labels" , doc.SelectSingleNode("//labels")?.InnerText.Trim()  ?? ""},
                        {"version" , doc.SelectSingleNode("//version")?.InnerText.Trim() ?? ""},
                        {"summary" , doc.SelectSingleNode("//summary")?.InnerText.Trim() ?? ""},
                        {"content" , src  ?? ""}
                    };
               
                collection.ReplaceOneAsync(
                    filter: new BsonDocument("issuekey", item),
                    options: new ReplaceOptions { IsUpsert = true },
                    replacement: post); 
            } 
            ctx.SessionDriver.Dispose(); 
            // task runner 
        }

        [TestMethod]
        public void SysConfig_Configs()
        {
            var configuration = new UnitTestManager().Configuration;
            var mock = new Mock<ILogger<ContextProvider>>();
            ILogger<ContextProvider> logger = mock.Object;
            IBScriptParser bomScriptParser = new BScriptParser();
      
            SessionContext ctx = new SessionContext();
            ctx.SessionDriver = new SessionDriver(configuration, logger, bomScriptParser, "driver:BOM.CORE.SessionDriver, BOM.CORE;https://localhost/login.aspx;s:UserName,Bill-D-Robertson;s:Password,Password;c:LoginButton;c:Accept;");
            ctx.SessionDriver.Connect();
            new ClickByContent("li.rtsLI", ".*CIO 2021 Q2.*", true).Execute(ctx); 
            ctx.SessionDriver.Pause(1000).Click("_Launch").Pause(1000);
            new SetOption("ddl_Sections", 4 ).Execute(ctx);
            //IWebElement[] IWebElements = new List<IWebElement>().ToArray(); 
            IList<IWebElement> inputs = ctx.SessionDriver.Driver.FindElements(By.CssSelector("tr[id*='ctl00__'] *[id*='EditButton']")); 
            List<string> ids = new List<string>();
            foreach (var item in inputs) ids.Add(item.GetAttribute("id"));
            foreach (var item in ids)
            {
                new Click($"*[id$='{item}']").Execute(ctx);
                new NaiveFormFill(".rgMasterTable").Execute(ctx);
                new Click("UpdateButton").Execute(ctx);
                ctx.SessionDriver.Pause(200);
            }

            ctx.SessionDriver.Dispose();

            // task runner 
        }
        [TestMethod]
        public void TestMethod1()
        {
            BOM.CORE.BTask task = new BOM.CORE.BTask();
            task.Name = "unittest";
            task.Context = "unittest";
            task.TaskSteps.Add(new BOM.CORE.TaskStep("Keys", new string[] { "Username", "user" }));
            task.TaskSteps.Add(new BOM.CORE.TaskStep("Keys", new string[] { "Password", "pass" }));

            SessionContext ctx = new SessionContext();
            ctx.SessionDriver.Connect(); 

            // task runner 
            foreach (var taskstep in task.TaskSteps)
            {
                Type tCmd = Type.GetType(taskstep.Cmd);  
                ParameterInfo[] PI = tCmd.GetConstructors()[0].GetParameters();
                List<object> oparms = new List<object>();
                int parmcnt = 0;
                foreach (ParameterInfo parm in PI)
                {
                    string value = taskstep.Args[parmcnt];
                    parmcnt++;
                    if (parm.ParameterType.Name.Contains("Int"))
                        oparms.Add(Convert.ToInt32(value));
                    else if (parm.ParameterType.Name.Contains("Bool"))
                        oparms.Add(Convert.ToBoolean(value));
                    else
                        oparms.Add(value);
                }
                ICommand obj = (ICommand)Activator.CreateInstance(tCmd, oparms);
                obj.Execute(ctx); 
            }
            // task runner 
        }
    }
}
