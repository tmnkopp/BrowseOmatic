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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TelerikCommands;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnitTests
{
    [TestClass]
    public class TelerikTests
    {
 
        List<BTask> tasks = new List<BTask>();
        ICommand cmd; 
 
        [TestMethod]
        public void Prepop_Submits()
        { 
            BTask task = new BTask("prepop", "dayadmin");
            task.TaskSteps.Add(new TaskStep("Url", new string[] { "~/Maintenance/Authoring/Prepopulate.aspx" }));

            tasks.Add(task); 
            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);

            WriteTasks(tasks);
        } 
        [TestMethod]
        public void User_Updates()
        {
            BTask task = new BTask("USER", "dayadmin");
            task.TaskSteps.Add(new TaskStep("OpenTab", new string[] { "~/UserAccessNew/SelectUser.aspx" }));
            task.TaskSteps.Add(new TaskStep("Key", new string[] { "_WebTextEdit1", "ll-d-rob" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "_btn_Run" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "_link_UserID" }));
            task.TaskSteps.Add(new TaskStep("SwitchTo", new string[] { "-1" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "input[id='ctl00_ctl00_contentPh_RightBody_ContentPlaceHolder1_fv_Profile_btn_Edit']" }));
            tasks.Add(task);

            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);

            WriteTasks(tasks); 
        }   
        [TestMethod]
        public void Rand_Submits()
        {
            string rnd = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 1).Select(s => s[new Random().Next(s.Length)]).ToArray());
            Assert.IsNotNull(rnd);
        }
        private void WriteTasks(List<BTask> tasks)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            File.WriteAllText($"c:\\bom\\unittest\\output.yaml", serializer.Serialize(tasks), Encoding.Unicode);
        }
    } 
}
 