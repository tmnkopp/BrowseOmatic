using BOM.CORE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Moq; 
namespace UnitTests
{
    [TestClass]
    public class ProcessorTests
    {
        [TestMethod]
        public void ProcessUrlProvider_Processes()
        {
            var configuration = new TestServices().Configuration; 
            var mock = new Mock<ILogger<ConfigTaskProvider>>();
            ILogger<ConfigTaskProvider> logger = mock.Object; 
            var task = new ConfigTaskProvider(configuration, logger).Items.Where((t) => t.Name.Contains("unittest")).FirstOrDefault();

            var mockp = new Mock<ILogger<ContextProvider>>();
            ILogger<ContextProvider> loggerp = mockp.Object;

            var context = new ContextProvider(configuration, loggerp).Get("unittest");
             
            Assert.IsNotNull(context);
        }

        [TestMethod]
        public void PromptProcess_Processes()
        {
            var configuration = new TestServices().Configuration;

            var mock = new Mock<ILogger<ConfigTaskProvider>>(); 
            ILogger<ConfigTaskProvider> logger = mock.Object;
            var task = new ConfigTaskProvider(configuration, logger)
                .Items.Where((t) => t.Name.Contains("unittest")).FirstOrDefault();

            var mockp = new Mock<ILogger<ContextProvider>>();
            ILogger<ContextProvider> loggerp = mockp.Object;
             
            var context = new ContextProvider(configuration,  loggerp).Get("unittest");

            // task -c rtime 
            Assert.IsNotNull(context); 
        } 
    } 
}
