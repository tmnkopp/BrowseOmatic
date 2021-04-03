using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using BOM;
using BOM.CORE;
using Moq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace UnitTests
{

    [TestClass]
    public class TaskRunner
    {
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
