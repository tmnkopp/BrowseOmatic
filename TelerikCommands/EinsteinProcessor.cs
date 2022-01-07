using BOM.CORE;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelerikCommands
{
    [CommandMeta(Context: "csagency")]
    public class EinsteinProcess : ICommand
    {
        int _sectionFrom = 0;
        int _sectionTo = 0;
        int _records = 1;
        private int _pause = 200;
        public EinsteinProcess(int SectionFrom, int SectionTo, int Records)
        {
            _sectionFrom = SectionFrom;
            _sectionTo = SectionTo;
            _records = Records;
        }
        public void Execute(ISessionContext ctx)
        {
            var dvr = ctx.SessionDriver;
            dvr.Create();
            IList<IWebElement> elements = ctx.SessionDriver.Driver.FindElements(By.CssSelector("*[id*='_Surveys'] li"));
            foreach (IWebElement element in elements)
            {
                if (element.Text.ToUpper().Contains($"EINSTEIN"))
                {
                    element.Click();
                    break;
                }
            }
            dvr.Pause(550).Click("_hl_Launch"); 
            for (int i = _sectionFrom; i <= _sectionTo; i++)
            {
                dvr.Pause(_pause);
                SelectElement sections = new SelectElement(dvr.Select("ctl00_ddl_Sections"));
                sections.SelectByIndex(i); 
                int recs = 0;
                while (recs < _records)
                {
                    dvr.Pause(_pause).Click("AddNewRecordButton_input");
                    ControlPopulate.GenericForm(ctx);
                    dvr.Pause(_pause).Click("_PerformInsertButton");
                    recs++;
                }

                try
                {
                    dvr.Pause(_pause).Click("_EditButton");
                    ControlPopulate.GenericForm(ctx);
                    dvr.Pause(_pause).Click("_UpdateButton");
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"EinsteinProcess: {ex.Message}");
                }
                try
                {
                    dvr.Driver.FindElement(By.CssSelector("a[title*='delete']")).Click();
                    IAlert alert = dvr.Pause(550).Driver.SwitchTo().Alert();
                    alert.Accept();
                }
                catch (Exception ex)
                { 
                    Console.WriteLine($"EinsteinProcess: {ex.Message}");
                } 
            }
        }
    }
}
