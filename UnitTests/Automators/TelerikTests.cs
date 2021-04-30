using BOM.CORE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TelerikCommands;

namespace UnitTests
{
    [TestClass]
    public class TelerikTests
    {
        [TestMethod]
        public void FFormTests_Closer()
        {
            var ctx = Session.Context("csagency");
            var dvr = ctx.SessionDriver;
            new ClickByContent("li.rtsLI", ".*IG.*2021.*", true).Execute(ctx);
            dvr.Pause(900).Click("_Launch");
            //new SetOption("ddl_Sections", 5);
            //new FismaForm(1, ".table").Execute(ctx); 

        }
        [TestMethod]
        public void TelerikTests_Closer()
        {
            var ctx = Session.Context("csagency"); 
            var dvr = ctx.SessionDriver;  
            new ClickByContent("li.rtsLI", ".*BOD.*2021.*", true).Execute(ctx);
            dvr.Pause(900).Click("_Launch"); 
            for (int section = 0; section <=3 ; section++)
            {
                int attempts = 0;
                new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(section);
                while (attempts < 2)
                { 
                    dvr.Click("btnEdit");
                    new RadFormFill(".table").Execute(ctx);
                    new NaiveFormFill(".table").Execute(ctx);
                    dvr.Click("btnSave").Pause(250);
                    attempts++;
                } 
            } 
        }
    } 
}
 