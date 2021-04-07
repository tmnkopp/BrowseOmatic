using System;
using System.Collections.Generic;
using System.Text;
using BOM.CORE;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TelerikCommands
{
    public class FismaForm : ICommand
    {
        private int section = 0;
        public FismaForm(int Section)
        {
            this.section = Section;
        }
        public void Execute(ISessionContext ctx)
        { 
            var dvr = ctx.SessionDriver;
            if (section > 0)  new SelectElement(dvr.Select("ctl00_ddl_Sections")).SelectByIndex(section); 
            dvr.Click("btnEdit"); 
            new RadFormFill("").Execute(ctx);
            dvr.Click("btnSave");
        }
    }
}
