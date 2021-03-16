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
        public void PromptProcess_Processes()
        {
            var configuration = new UnitTestManager().Configuration;

            var mock = new Mock<ILogger<ConfigTaskProvider>>(); 
            ILogger<ConfigTaskProvider> logger = mock.Object;
            var task = new ConfigTaskProvider(configuration, logger)
                .Items.Where((t) => t.Name.Contains("unittest")).FirstOrDefault();

            var mockp = new Mock<ILogger<ContextProvider>>();
            ILogger<ContextProvider> loggerp = mockp.Object;

            IBScriptParser bomScriptParser = new BScriptParser();

            var context = new ContextProvider(configuration, bomScriptParser, loggerp)
                .Items.Where((t) => t.Name.Contains("unittest")).FirstOrDefault();

            // task -c rtime 
            Assert.IsNotNull(context); 
        }
        [TestMethod]
        public void TaskProcessor_Processes()
        { 
            var configuration = new UnitTestManager().Configuration;

            var mock = new Mock<ILogger<ConfigTaskProvider>>();
            ILogger<ConfigTaskProvider> logger = mock.Object;  
       
            var task = (from t in new ConfigTaskProvider(configuration, logger).Items 
                        where t.Name.Contains("unittest") select t).FirstOrDefault();
 
            var taskProcessor = new TaskProcessor(task, logger); 
            Assert.IsNotNull(taskProcessor); 
        }
    }
 


}
