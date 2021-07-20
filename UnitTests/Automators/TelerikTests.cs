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
        }
        [TestMethod]
        public void NaiveInput_Submits()
        { 
            var ctx = Session.Context("csagency"); 
            new OpenTab("https://localhost/Maintenance/ManageSolarWinds.aspx").Execute(ctx);
        } 
        [TestMethod]
        public void EmailDistributionExternal_Submits()
        {
            var ctx = Session.Context("csadmin");
            var dvr = ctx.SessionDriver;
            dvr.Pause(200);
            new OpenTab("https://localhost/Maintenance/PickListMaint.aspx").Execute(ctx);
            // new OpenTab("https://localhost/Maintenance/Authoring/FormAuthDefault.aspx").Execute(ctx);
            // new OpenTab("https://localhost/Maintenance/Authoring/Prepopulate.aspx").Execute(ctx);
        }
        [TestMethod]
        public void AdminEO_Updates()
        {
            var ctx = Session.Context("dayadmin"); //  dayadmin    csadmin
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*EO.*2021.*", true).Execute(ctx);
        }
        [TestMethod]
        public void CIO_RMA_Submits()
        {
            var ctx = Session.Context("dayagency");
            var dvr = ctx.SessionDriver; // 1
            new ClickByContent("li.rtsLI", ".*BOD.*2020.*", true).Execute(ctx);
            //dvr.Pause(500).Click("hl_Launch").Pause(500);
            //new SetOption("ddl_Sections", 14);
        }
        [TestMethod]
        public void User_Updates()
        {
            var ctx = Session.Context("dayadmin"); //  dayadmin    csadmin
            var dvr = ctx.SessionDriver; 
  
            new OpenTab("https://dayman.cyber-balance.com/CyberScopeBranch/UserAccessNew/SelectUser.aspx").Execute(ctx);
            dvr.Pause(500).SendKeys("_WebTextEdit1", "ll-d-rob").Click("_btn_Run").Click("_link_UserID").Pause(550);
            var handles = dvr.Driver.WindowHandles; 
            dvr.Driver.SwitchTo().Window(handles[handles.Count - 1]); 
            dvr.Pause(1500).Click("_btn_Edit"); 
        }
        [TestMethod]
        public void Assessment_Submits()
        {
            var ctx = Session.Context("csagency"); //  dayman    csagency
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*18-02.*Remediation.*", true).Execute(ctx);
        
            new ClickByContent("a", ".*Manage New Assessment.*", true).Execute(ctx);
            var handles = dvr.Driver.WindowHandles;
            dvr.Driver.SwitchTo().Window(handles[handles.Count - 1]); 
        } 
        [TestMethod]
        public void HVA_ANNUAL_Submits()
        {
            var ctx = Session.Context("csagency");
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*BOD.*Annual 2020.*", true).Execute(ctx);
            dvr.Pause(250).Click("ctl16_hl_Launch").Pause(1250);
            for (int i = 0; i <= 2; i++)
            { 
                new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(i);
                dvr.Click("btnEdit");
                new RadFormFill(".table").Execute(ctx);
                dvr.Click("btnSave").Pause(150); 
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
        public void BOD_Sensitive19_Submits()
        { 
            var ctx = Session.Context("csagency");
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*BOD.*2021.*", true).Execute(ctx);
            dvr.Pause(900).Click("ctl14_hl_Launch");
            new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(3);
            dvr.Click("_btnEdit");
  
            // new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(6);
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
            // new FismaForm(1, ".table").Execute(ctx);
            // new FismaForm(2, ".table").Execute(ctx);
            // new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(6); 
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
 