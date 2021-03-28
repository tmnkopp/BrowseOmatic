using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    public interface ICommand
    {
        public void Execute(ISessionContext SessionContext);
    }
    /*
    public class MyCustomAutomator : ICommand
    {
        private string MyConfigArg = "";
        private int MyConfiArg2 = 0;
        public MyCustomAutomator(string MyConfigArg, int MyConfiArg2)
        {
            this.MyConfigArg = MyConfigArg;
            this.MyConfiArg2 = MyConfiArg2;
        }
        public void Execute(ISessionContext SessionContext)
        {
            var ctx = SessionContext;
            ctx.SessionDriver
                .GetUrl($"http://domainname.com/page-number/{MyConfiArg2.ToString()}")
                .Click("Element")
                .SendKeys("Element", "Type This") 
                .Click("Element")
                .SendKeys($"{MyConfigArg}", $"Type This"); 
        }
    }
    */
}
