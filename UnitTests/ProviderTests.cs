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
        public void InputDefaultProvider_Provides() {
            var configuration = new TestServices().Configuration;
            var sections = configuration.GetSection("InputDefaults").GetChildren().AsEnumerable();

            List<InputDefault> InputDefaults = new List<InputDefault>();
            foreach (var item in sections)
            {
                InputDefault id = new InputDefault(item.Key);
                foreach (var idi in item.GetChildren())
                {
                    InputDefaultItem inputitem = new InputDefaultItem(id,idi.Key, idi.Value); 
                    Console.Write($"{idi}");
                }
                try
                {
                    InputDefaults.Add(new InputDefault
                    {
                        ID = item.Key
                    });
                }
                catch (Exception e)
                { 
                    Console.Write($" {e.Message} {e.StackTrace} ");
                }
            }
            Assert.IsNotNull(InputDefaults);
            //return InputDefaults.ToList();
        }

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
        public void Provider_Provides()
        {
            var configuration = new TestServices().Configuration;
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
            var configuration = new TestServices().Configuration;
            var mock = new Mock<ILogger<ConfigTaskProvider>>();
            ILogger<ConfigTaskProvider> logger = mock.Object;

            var provider = new ConfigTaskProvider(configuration, logger);  
            var task = provider.Items.Where((t) => t.Name.Contains("unittest")).FirstOrDefault();
            Assert.IsNotNull(task);
        }
    }
}
