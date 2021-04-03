using BOM.CORE;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel;

namespace UnitTests
{
 
    [TestClass]
    public class ProviderTests
    {

        [TestMethod]
        public void YMLProvider_Provides()
        {
            var yaml = new YamlStream();
            using (TextReader tr = File.OpenText(@"C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic\config.yaml"))
                yaml.Load(tr);

            List<BTask> tasks = new List<BTask>();

            var root = (YamlMappingNode)yaml.Documents[0].RootNode;
            var ytasks = (YamlSequenceNode)root.Children[new YamlScalarNode("tasks")];

            foreach (YamlMappingNode ytask in ytasks)
            {
                string name = ytask[new YamlScalarNode("task")].ToString();
                string context = ytask[new YamlScalarNode("context")].ToString();
                var ysteps = (YamlSequenceNode)ytask.Children[new YamlScalarNode("steps")];
                var TaskSteps = new List<TaskStep>();
                foreach (YamlMappingNode step in ysteps)
                {
                    var argument = step.Children.FirstOrDefault().Value;
                    List<string> args = new List<string>();
                    if (argument.GetType() == typeof(YamlSequenceNode))
                    {
                        var ars = ((YamlSequenceNode)step.Children.FirstOrDefault().Value).Children;
                        args = (from n in ars select ((YamlScalarNode)n).Value).ToList();
                    }
                    if (argument.GetType() == typeof(YamlScalarNode))
                    {
                        args.Add(((YamlScalarNode)argument).Value.ToString());
                    }
                    string cmd = step.Children.FirstOrDefault().Key.ToString();
                    TaskSteps.Add(new TaskStep(cmd, args.ToArray()));
                }
                tasks.Add(
                      new BTask() { Name = name, Context = context, TaskSteps = TaskSteps }
                );
            }
            Assert.IsTrue(tasks.Count > 0); 
        }
        [TestMethod]
        public void ContextProvider_ProvidesSessionDriver()
        {
            var configuration = new UnitTestManager().Configuration;
            var mock = new Mock<ILogger<ContextProvider>>();
            ILogger<ContextProvider> logger = mock.Object;
            IBScriptParser bomScriptParser = new BScriptParser();
            var provider = new ContextProvider(
                  configuration
                , bomScriptParser
                , logger);
            var isnull = false;
            foreach (var item in provider.Items)
            {
                if (item.SessionDriver == null)
                {
                    isnull = true;
                    break;
                }
            } 
            Assert.IsNotNull(isnull);
        }

        [TestMethod]
        public void BomScriptParser_Parses()
        {
            var strparse = "driver:BOM.CORE.SessionDriver, BOM.CORE;https://dayman.cyber-balance.com/jira/login.jsp; s:username,userexpected; s:password,expected; c:submit;";
            var parser = new BScriptParser();
            BScriptParseResult result = new BScriptParseResult();
            foreach (var item in parser.Parse(strparse))
            {
                result = item;
            };
            Assert.IsTrue(result.QualifiedCommand == "Click"); 
        }

        [TestMethod]
        public void Provider_Provides()
        {
            var configuration = new UnitTestManager().Configuration;
            var sections = configuration.GetSection("contexts").GetChildren().AsEnumerable();
            var contexts = from s in sections
                           let name = s["name"]
                           let conn = s["conn"] 
                           select new SessionContext
                           {
                               Name = name
                           }; 
            Assert.IsNotNull(contexts);
        } 

        [TestMethod]
        public void TaskProvider_Provides()
        { 
            var configuration = new UnitTestManager().Configuration;
            var mock = new Mock<ILogger<ConfigTaskProvider>>();
            ILogger<ConfigTaskProvider> logger = mock.Object;

            var provider = new ConfigTaskProvider(configuration, logger);  
            var task = provider.Items.Where((t) => t.Name.Contains("unittest")).FirstOrDefault();
            Assert.IsNotNull(task);
        }
    }
}
