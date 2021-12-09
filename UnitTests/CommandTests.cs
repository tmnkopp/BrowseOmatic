using BOM.CORE;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TelerikCommands;

namespace UnitTests
{ 
    [TestClass]
    public class CommandTests
    {

        [TestMethod]
        public void valueConfig_Fills()
        {
            string val = "\\dx\\dxxx\\dxx";
            StringBuilder sb = new StringBuilder();
            var vals = val.Split(new string[] { "\\d" }, StringSplitOptions.None);
            for (int i = 1; i < vals.Length; i++)
            {
                sb.Append($"9{vals[i]}");
            } 
            var st = $"{vals[0]}{sb.ToString()}";
            Assert.IsNotNull(st);
        }

        [TestMethod]
        public void RadFormFill_Fills()
        {
            var config = new TestServices().Configuration; 
            ILogger<ContextProvider> logger = new Mock<ILogger<ContextProvider>>().Object;
            
            ISessionDriver dvr = new SessionDriver( config , logger ); 
            ISessionContext ctx = new SessionContext();
            ctx.Name = "unittest";
            ctx.SessionDriver = dvr;
            ctx.configContext = new BomConfigContext();
            ctx.configContext.root = "https://localhost/";
            ctx.configContext.conn = "driver:BOM.CORE.SessionDriver, BOM.CORE;https://localhost/login.aspx;s:UserName,Bill-D-Robertson;s:Password,P@ssword1;c:LoginButton;c:Accept;";

            ctx.SessionDriver.Connect(ctx.configContext.conn); 
            new ClickByContent("li.rtsLI", ".*EINS.*", true).Execute(ctx); 
            new Click("_Launch").Execute(ctx);
            new Click("_AddNewRecordButton").Execute(ctx);
            new RadFormFill(".rgEditRow").Execute(ctx);  

            Assert.IsNotNull(ctx);

        }
        [TestMethod]
        public void OpenTab_Opens()
        {
            var config = new TestServices().Configuration; 
            ILogger<ContextProvider> logger = new Mock<ILogger<ContextProvider>>().Object;
 
            ICommand cmd = new OpenTab("https://gist.github.com/safebear/a550c4094811993f3c223e1d2f8a8eb5");
            ICommand url = new Url("https://gist.github.com/safebear/a550c4094811993f3c223e1d2f8a8eb5");
            ISessionDriver dvr = new SessionDriver(
                config
                , logger 
                ); 
            ISessionContext ctx = new SessionContext();
            ctx.Name = "unittest";
            ctx.SessionDriver = dvr;
            ctx.SessionDriver.Connect(ctx.configContext.conn);
            url.Execute(ctx);
            cmd.Execute(ctx);

            Assert.IsNotNull(ctx);

        }
    }
}
