using BOM.CORE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TelerikCommands;

namespace UnitTests
{
    [TestClass]
    public class TelerikTests
    {
        [TestMethod]
        public void admin_Submits()
        {
            var ctx = Session.Context("csadmin");
            var dvr = ctx.SessionDriver;

            new ClickByContent("li.rtsLI", ".*Solar.*", true).Execute(ctx);
            dvr.Pause(250).Click("_ctl04_lnkAdmin").Pause(1250);

            new OpenTab("https://localhost/OMBhome.aspx").Execute(ctx);
            new ClickByContent("li.rtsLI", ".*Solar.*", true).Execute(ctx);
            dvr.Pause(250).Click("_ctl16_lnkAdmin").Pause(1250);

            new OpenTab("https://localhost/OMBhome.aspx").Execute(ctx);
            new ClickByContent("li.rtsLI", ".*Solar.*", true).Execute(ctx);
            dvr.Pause(250).Click("_ctl06_lnkAdmin").Pause(1250);

            new OpenTab("https://localhost/UserAccessNew/SelectUser.aspx").Execute(ctx);
            dvr.Pause(500).SendKeys("_WebTextEdit1", "bill").Click("_btn_Run");

        }
        [TestMethod]
        public void NaiveInput_Submits()
        { 
            var ctx = Session.Context("csagency");
            var dvr = ctx.SessionDriver;
            new OpenTab("https://localhost/Maintenance/ManageSolarWinds.aspx").Execute(ctx);
        }
        [TestMethod]
        public void Test_Validates()
        {
            var ctx = Session.Context("dayagency");
            var dvr = ctx.SessionDriver; 
            new ClickByContent("li.rtsLI", ".*Solar.*", true).Execute(ctx);
            dvr.Pause(1250); 
            new ClickByContent("a", ".*MANAGE SOLARWINDS.*", true).Execute(ctx); 
        }
        [TestMethod]
        public void Stage_Validates()
        {
            var ctx = Session.Context("dayadmin");
            var dvr = ctx.SessionDriver;
      
            new ClickByContent("li.rtsLI", ".*Solar.*", true).Execute(ctx);
            dvr.Pause(250).Click("_ctl04_lnkAdmin").Pause(1250);

            new OpenTab("https://dayman.cyber-balance.com/CyberScopeBranch/OMBhome.aspx").Execute(ctx);
            new ClickByContent("li.rtsLI", ".*Solar.*", true).Execute(ctx);
            dvr.Pause(250).Click("_ctl16_lnkAdmin").Pause(1250);

            new OpenTab("https://dayman.cyber-balance.com/CyberScopeBranch/OMBhome.aspx").Execute(ctx);
            new ClickByContent("li.rtsLI", ".*Solar.*", true).Execute(ctx);
            dvr.Pause(250).Click("_ctl06_lnkAdmin").Pause(1250);

            new OpenTab("https://dayman.cyber-balance.com/CyberScopeBranch/UserAccessNew/SelectUser.aspx").Execute(ctx);
            dvr.Pause(500).SendKeys("_WebTextEdit1", "bill").Click("_btn_Run");

        }
        [TestMethod]
        public void Naive_Submits()
        {
            var ctx = Session.Context("csagency");
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*Solar.*", true).Execute(ctx);
            dvr.Pause(250).Click("10_hl_Launch").Pause(1250);
        }


        [TestMethod]
        public void CloudTests_Submits()
        { 
            var ctx = Session.Context("csagency");
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*BOD.*2021.*", true).Execute(ctx);
            dvr.Pause(900).Click("ctl14_hl_Launch");
            for (int i = 3; i < 5; i++)
            {
                new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(i);
                dvr.Click("btnEdit");
                new CloudGrid(".table").Execute(ctx);
                dvr.Click("btnSave").Pause(250);
            } 
        }
        [TestMethod]
        public void FFormTests_Closer()
        {
            var ctx = Session.Context("csagency");
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*IG.*2021.*", true).Execute(ctx);
            dvr.Pause(900).Click("_Launch");
            new SetOption("ddl_Sections", 0);
            new FismaForm(1, ".table").Execute(ctx); 

        }
        [TestMethod]
        public void Validater_Validates()
        {
            var ctx = Session.Context("dayman");
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*BOD.*2021.*", true).Execute(ctx);
            dvr.Pause(900).Click("08_hl_Launch");  
            new FismaForm(1, ".table").Execute(ctx);
            new FismaForm(2, ".table").Execute(ctx);
            new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(6);

        }
        [TestMethod]
        public void TelerikTests_Closer()
        {
            var ctx = Session.Context("dayman"); 
            var dvr = ctx.SessionDriver;  
            new ClickByContent("li.rtsLI", ".*BOD.*2021.*", true).Execute(ctx);
            dvr.Pause(900).Click("ctl14_hl_Launch");
            for (int i = 0; i <= 2; i++)
            {
                int j = 0;
                while (j < 1)
                {
                    new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(i);
                    dvr.Click("btnEdit");
                    new RadFormFill(".table").Execute(ctx);
                    dvr.Click("btnSave").Pause(150);
                    j++;
                } 
            }
            for (int i = 3; i < 5; i++)
            {
                new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(i);
                dvr.Click("btnEdit");
                new CloudGrid(".table").Execute(ctx);
                dvr.Click("btnSave").Pause(150);
            }
            new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(5);
        }
        [TestMethod]
        public void Rand_Submits()
        {
            string rnd = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 1).Select(s => s[new Random().Next(s.Length)]).ToArray());
            Assert.IsNotNull(rnd);
        }
    } 
}
 