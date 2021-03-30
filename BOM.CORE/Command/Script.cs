using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOM.CORE 
{ 
    public class Script : ICommand
    {
        string script = ""; 
        public Script(string script)
        {
            this.script = script;
            foreach (var item in new string[] { "\n", "\r", "\t" })
                this.script = this.script.Replace(item, "");
 
        }
        public void Execute(ISessionContext ctx)
        { 
            var driver = ctx.SessionDriver.Driver; 
            try
            {
                ctx.SessionDriver.Pause(20);
                ((IJavaScriptExecutor)driver).ExecuteScript($"{this.script}");
            }
            catch (Exception ex)
            {
                Console.Write($" Invalid Javascript {ex.Message} {this.script}"); 
            }
           
        }
    }  
}
