using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            var paths = configuration.GetSection("paths");
            var yamltasks = paths.GetSection("yamltasks").Value; 

            var yaml = new YamlStream();
            using (TextReader tr = File.OpenText(yamltasks))
                yaml.Load(tr);

            var root = (YamlMappingNode)yaml.Documents[0].RootNode;
            var ytasks = (YamlSequenceNode)root.Children[new YamlScalarNode("tasks")];
            string[] args = null;
            List<BTask> tasks = new List<BTask>();
            foreach (YamlMappingNode ytask in ytasks)
            {
                string name = ytask[new YamlScalarNode("task")].ToString();
                string context = ytask[new YamlScalarNode("context")].ToString();
                var ysteps = (YamlSequenceNode)ytask.Children[new YamlScalarNode("steps")];
                var TaskSteps = new List<TaskStep>();
                foreach (YamlMappingNode step in ysteps)
                {
                    var ars = ((YamlSequenceNode)step.Children.FirstOrDefault().Value).Children;
                    args = (from n in ars select ((YamlScalarNode)n).Value).ToArray();
                    string cmd = step.Children.FirstOrDefault().Key.ToString();
                    TaskSteps.Add(new TaskStep(cmd, args)); 
                }
                tasks.Add(
                      new BTask()  {  Name = name,  Context = context, TaskSteps = TaskSteps  }
                );
            } 
            return tasks.ToList();
        }
        #endregion 
    }
}
