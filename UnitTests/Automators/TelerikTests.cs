using BOM.CORE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace UnitTests
{
    [TestClass]
    public class TelerikTests
    {
        StringBuilder sb = new StringBuilder();
        ICommand cmd; 
        [TestMethod]
        public void admin_Submits()
        {
            var ctx = Session.Context("csadmin");
            var dvr = ctx.SessionDriver;   
        } 
        [TestMethod]
        public void CIO_RMA_Submits()
        {
            var ctx = Session.Context("dayagency");
            var dvr = ctx.SessionDriver; // 1
            new ClickByContent("li.rtsLI", ".*CIO.*Q3.*", true).Execute(ctx);
            dvr.Pause(500).Click("hl_Launch").Pause(1000);
            new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(1);
            dvr.Pause(1000).Click("_btnEdit"); 
        } 
        [TestMethod]
        public void User_Updates()
        {
            var ctx = Session.Context("dayadmin"); // dayadmin csadmin
            var dvr = ctx.SessionDriver; 
            new OpenTab("https://dayman.cyber-balance.com/CyberScopeBranch/UserAccessNew/SelectUser.aspx").Execute(ctx);
            dvr.Pause(500).SendKeys("_WebTextEdit1", "ll-d-rob").Click("_btn_Run").Click("_link_UserID").Pause(550);
            new SwitchTo(-1).Execute(ctx);
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
        public void Rand_Submits()
        {
            string rnd = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 1).Select(s => s[new Random().Next(s.Length)]).ToArray());
            Assert.IsNotNull(rnd);
        }
    } 
}
 