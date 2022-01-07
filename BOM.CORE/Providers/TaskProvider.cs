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
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BOM.CORE 
{

    public class TaskProvider : ISettingProvider<BTask> 
    {
        #region CTOR
        private readonly IConfiguration configuration;
        private readonly ILogger<ConfigTaskProvider> logger;
        public TaskProvider(
            IConfiguration Configuration,
            ILogger<ConfigTaskProvider> Logger)
        {
            configuration = Configuration;
            logger = Logger;
        }
        #endregion  
        #region Methods 
        public BTask Get(string ItemName)
        {
            var yamltasks = configuration.GetSection("paths:yamltasks")?.Value;
            var taskfile = $"{yamltasks}{ItemName}";
            if (!taskfile.EndsWith(".yaml"))
                taskfile += ".yaml";
             
            string yamlraw = "";
            using (TextReader tr = File.OpenText(taskfile))
                yamlraw = tr.ReadToEnd().Replace("tasks:", "");

            var deserializer = new DeserializerBuilder()
               .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();
            BTask task;
            try
            {
                task = deserializer.Deserialize<BTask>(yamlraw);
            }
            catch (Exception ex)
            {
                logger.LogError("paths:yamltasks : {o}", taskfile);
                logger.LogError("GetExecutingAssembly : {o}", Assembly.GetExecutingAssembly().Location);
                throw new Exception($"YamlStream Deserialize Failed: {ex.Message}");
            } 
            return task;
        }
        #endregion 
    }
}
