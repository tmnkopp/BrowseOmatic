using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOM.CORE
{
    public class SetOptionByText : ICommand
    {
        private string Element = "";
        private string option = ".*";
        public SetOptionByText( string Element, string Option)
        {
            this.Element = Element;
            this.option = Option;
        } 
        public void Execute(ISessionContext ctx)
        {
            var dvr = ctx.SessionDriver;
            ctx.SessionDriver.Pause(0);
            SelectElement sections = new SelectElement(dvr.Select(Element));
            if (sections != null)
            {
                try
                {
                    ctx.SessionDriver.Log.LogInformation("SetOptionByText {o}", Element); 
                    sections?.Options.Where(o => o.Text.Contains(this.option)).FirstOrDefault()?.Click();
                }
                catch (Exception e)
                {
                    ctx.SessionDriver.Log.LogWarning("SetOptionByText {o}", e.Message); 
                }
            }  
        }
    }
}
