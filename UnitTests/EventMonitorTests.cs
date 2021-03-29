using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class EventMonitorTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            ChromeOptions options = new ChromeOptions(); 
            var chromeDriverService = ChromeDriverService.CreateDefaultService(@"c:\bom");
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            options.AddArgument("log-level=3");
            var driver = new ChromeDriver(chromeDriverService, options);
            var result = driver.ExecuteChromeCommandWithResult("monitorEvents", new Dictionary<string, object>() { { "", "" } }); 
            Assert.IsNotNull(driver);
        }
    }
}
