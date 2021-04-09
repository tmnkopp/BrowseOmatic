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
