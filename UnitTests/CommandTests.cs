using BOM.CORE;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void OpenTab_Opens()
        {
            var config = new UnitTestManager().Configuration; 
            ILogger<ContextProvider> logger = new Mock<ILogger<ContextProvider>>().Object;
            IBScriptParser bomScriptParser = new BScriptParser();
            ICommand cmd = new OpenTab("https://gist.github.com/safebear/a550c4094811993f3c223e1d2f8a8eb5");
            ICommand url = new Url("https://gist.github.com/safebear/a550c4094811993f3c223e1d2f8a8eb5");
            ISessionDriver dvr = new SessionDriver(
                config
                , logger
                , bomScriptParser
                , "driver:BOM.CORE.SessionDriver, BOM.CORE;"
                ); 
            ISessionContext ctx = new SessionContext();
            ctx.Name = "unittest";
            ctx.SessionDriver = dvr;
            ctx.SessionDriver.Connect();
            url.Execute(ctx);
            cmd.Execute(ctx);

            Assert.IsNotNull(ctx);

        }
    }
}
