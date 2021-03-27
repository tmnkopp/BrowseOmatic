using BOM.CORE;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM
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
            var sd = ctx.SessionDriver;
            sd.Connect();
            IList<IWebElement> elements = ctx.SessionDriver.Driver.FindElements(By.CssSelector("*[id*='_Surveys'] li"));
            foreach (IWebElement element in elements)
            {
                if (element.Text.ToUpper().Contains($"EINSTEIN"))
                {
                    element.Click();
                    break;
                }
            }
            System.Threading.Thread.Sleep(400); 
            sd.Click("_hl_Launch"); 
            for (int i = _sectionFrom; i <= _sectionTo; i++)
            {
                sd.Pause(_pause);
                SelectElement sections = new SelectElement(sd.Select("ctl00_ddl_Sections"));
                sections.SelectByIndex(i);

                int recs = 0;
                while (recs < _records)
                {
                    sd.Pause(_pause).Click("AddNewRecordButton_input");
                    ControlPopulate.GenericForm(ctx);
                    sd.Pause(_pause).Click("_PerformInsertButton");
                    recs++;
                }
                sd.Pause(_pause).Click("_EditButton");
                ControlPopulate.GenericForm(ctx);
                sd.Pause(_pause).Click("_UpdateButton");
                sd.Driver.FindElement(By.CssSelector("a[title*='delete']")).Click();
                IAlert alert = sd.Pause(550).Driver.SwitchTo().Alert();
                alert.Accept();
            }
        }
    }
}
