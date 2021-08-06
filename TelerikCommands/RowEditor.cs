using System;
using System.Collections.Generic;
using System.Text;
using BOM.CORE;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TelerikCommands
{
    public class RowEditor : ICommand
    {
        private string container = "";
        public RowEditor(string Container)
        {
            this.container = Container;
        }
        public override string ToString()
        {
            return $"RowEditor ['{this.container}']";
        }
        public void Execute(ISessionContext ctx)
        { 
            var dvr = ctx.SessionDriver;
            IList<IWebElement> inputs = ctx.SessionDriver.Driver.FindElements(By.CssSelector("tr[id*='ctl00__'] *[id*='EditButton']"));
            List<string> ids = new List<string>();
            foreach (var item in inputs) ids.Add(item.GetAttribute("id"));
            foreach (var item in ids)
            {
                new Click($"*[id$='{item}']").Execute(ctx);
                new NaiveFormFill($"{this.container}").Execute(ctx);
                new Click("UpdateButton").Execute(ctx);
                ctx.SessionDriver.Pause(200);
            }
        }
    }
}
