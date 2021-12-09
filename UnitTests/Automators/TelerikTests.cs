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
        Dictionary<int, string> eins = new Dictionary<int, string>();
        List<BTask> tasks = new List<BTask>();
        ICommand cmd;
        public TelerikTests()
        {
            eins.Add(0, @"C:\Users\Tim\Downloads\Bill-D-Robertson_EinsteinPublicIP.xlsx");
            eins.Add(1, @"C:\Users\Tim\Downloads\Bill-D-Robertson_EinsteinUnannounced.xlsx");
            eins.Add(2, @"C:\Users\Tim\Downloads\Bill-D-Robertson_EinsteinTaps.xlsx");
            eins.Add(3, @"C:\Users\Tim\Downloads\Bill-D-Robertson_EinsteinVLAN.xlsx");
            eins.Add(4, @"C:\Users\Tim\Downloads\Bill-D-Robertson_EinsteinUnmonitoredTraffic.xlsx");
            eins.Add(5, @"C:\Users\Tim\Downloads\Bill-D-Robertson_EinsteinNAT.xlsx");
            eins.Add(6, @"C:\Users\Tim\Downloads\Bill-D-Robertson_EinsteinBGP.xlsx");

            //C:\Users\Tim\Downloads
        }
        [TestMethod]
        public void EINS_Submits()
        { 
            BTask task = new BTask("einstein_all_sections", "localagency");
            task.TaskSteps.Add(new TaskStep("ClickByContent", new string[] { "li.rtsLI", ".*EINST.*", "true" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "_Launch" }));
            for (int i = 0; i < 7; i++)
            {
                task.TaskSteps.Add(new TaskStep("SetOption", new string[] { "ddl_Sections", $"{i}" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "_btnEdit" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "_DeleteButton" }));
                task.TaskSteps.Add(new TaskStep("AcceptAlert", new string[] { "1000" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "AddNewRecordButton" }));
                task.TaskSteps.Add(new TaskStep("RadFormFill", new string[] { "*[class*='EinsteinGrid']" }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "a[id*='_PerformInsertButton']" })); 
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "*[id*='_rbtnDownload']" }));
                task.TaskSteps.Add(new TaskStep("Pause", new string[] { "1000" }));
                task.TaskSteps.Add(new TaskStep("Key", new string[] { "_fileUpload", eins[i] }));
                task.TaskSteps.Add(new TaskStep("Click", new string[] { "_cmdUpload, _btnSave" }));  
                tasks.Add(task);
            }
            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);
            Utils.WriteTasks(tasks);
        }
 
        [TestMethod]
        public void Assmt_All()
        {
            BTask task = new BTask("assmt_all_tabs", "localagency");
            task.TaskSteps.Add(new TaskStep("ClickByContent", new string[] { "li.rtsLI", ".*BOD 18-02 Rem.*", "true" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "_lnkAddAssm" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "*[id*='AssmtGrid_ctl00_ctl04' ] .rgExpandIcon" }));
            task.TaskSteps.Add(new TaskStep("OpenTab", new string[] { "~/ReporterHome.aspx" }));
            task.TaskSteps.Add(new TaskStep("OpenTab", new string[] { "~/ReporterHome.aspx" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "_Launch" }));
            task.TaskSteps.Add(new TaskStep("SetOption", new string[] { "ddl_Sections", "0" }));
            task.TaskSteps.Add(new TaskStep("OpenTab", new string[] { "" }));
            task.TaskSteps.Add(new TaskStep("SetOption", new string[] { "ddl_Sections", "1" }));
            task.TaskSteps.Add(new TaskStep("OpenTab", new string[] { "" }));
            task.TaskSteps.Add(new TaskStep("SetOption", new string[] { "ddl_Sections", "2" }));
            task.TaskSteps.Add(new TaskStep("SwitchTo", new string[] { "0" }));
            tasks.Add(task);

            CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            processor.Process(task);

            Utils.WriteTasks(tasks);
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

            Utils.WriteTasks(tasks);
        }   
        [TestMethod]
        public void Rand_Submits()
        {
            string rnd = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 1).Select(s => s[new Random().Next(s.Length)]).ToArray());
            Assert.IsNotNull(rnd);
        }
 
    } 
}
 