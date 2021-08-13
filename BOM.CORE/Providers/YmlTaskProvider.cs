using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace BOM.CORE
{

    public class YmlTaskProvider: IAppSettingProvider<BTask>
    {
        #region CTOR
        private readonly IConfiguration configuration;
        private readonly ILogger<ConfigTaskProvider> logger;
        public YmlTaskProvider(
            IConfiguration Configuration,
            ILogger<ConfigTaskProvider> Logger)
        { 
            configuration = Configuration;
            logger = Logger;
        }
        #endregion 
        #region PROPS
        public IEnumerable<BTask> Items
        {
            get { return GetItems(); }
        }
        #endregion
        #region Methods 
        private IEnumerable<BTask> GetItems()
        { 
            var yamltasks = configuration.GetSection("paths:yamltasks")?.Value; 
            if (yamltasks == null) {  
                logger.LogWarning("task path null : {o}", yamltasks);
                logger.LogWarning("GetExecutingAssembly : {o}", Assembly.GetExecutingAssembly().Location);
                throw new Exception("Invalid task path enviornment");
            }
            string bomroot = Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User).ToLower().Replace("bom.exe", "");
            if (string.IsNullOrEmpty(yamltasks)) 
                yamltasks = $"{bomroot}unittest.yaml";
            if (!yamltasks.Contains(":\\"))
                yamltasks = $"{bomroot}{yamltasks}";
            if (!yamltasks.EndsWith(".yaml"))
                yamltasks += ".yaml";
     
            List<BTask> tasks = new List<BTask>();
            var yaml = new YamlStream();
            try
            {
                using (TextReader tr = File.OpenText(yamltasks))
                    yaml.Load(tr);
            }
            catch (Exception )
            {
                logger.LogError("paths:yamltasks : {o}", yamltasks);
                logger.LogError("GetExecutingAssembly : {o}", Assembly.GetExecutingAssembly().Location);
                throw new Exception("YamlStream Open Fail");
            }
             
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
            return tasks.ToList();
        }
        #endregion 
    }
}
