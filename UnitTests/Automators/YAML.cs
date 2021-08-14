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
    public class YAML
    {
        StringBuilder sb = new StringBuilder();
        ICommand cmd;
        [TestMethod]
        public void SAOP_Submits()
        {
            sb.Clear();
            sb.AppendLine("  - task: SAOP_Submits");
            sb.AppendLine("    context: csagency");
            sb.AppendLine("    steps: ");

            var ctx = Session.Context("csagency"); //  dayman    csagency
            var dvr = ctx.SessionDriver;

            cmd = new ClickByContent("li.rtsLI", ".*SAOP.*21.*", true);
            cmd.Execute(ctx);
            sb.AppendLine($"    - {cmd.ToString()}");
  
            dvr.Pause(500);
            cmd = new Click("hl_Launch");
            cmd.Execute(ctx);
            sb.AppendLine($"    - {cmd.ToString()}");

            SelectElement select = new SelectElement(dvr.Select("ddl_Sections"));
            var cnt = select.Options.Count();
            for (int i = 0; i < cnt; i++)
            {
                new SetOption("ddl_Sections", i).Execute(ctx);
                ((IJavaScriptExecutor)ctx.SessionDriver.Driver).ExecuteScript($"document.title = '{i}';");
                
                if (dvr.ElementExists("table[id*='InvGrid']") || dvr.ElementExists("table[id*='PIAGrid']"))  {
                    sb.AppendLine($"    - SetOption: ['ddl_Sections',{i.ToString()}]");
                    cmd = new InvGrid(".table");
                    cmd.Execute(ctx);
                    sb.AppendLine($"    - {cmd.ToString()}"); 
                }else{
                    cmd = new FismaForm(i, ".table");
                    cmd.Execute(ctx);
                    sb.AppendLine($"    - {cmd.ToString()}"); 
                } 
            }
            dvr.Dispose(); 
        }  
    } 
}
 