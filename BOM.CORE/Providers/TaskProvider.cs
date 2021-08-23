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
    public class TaskProvider : IAppSettingProvider<BTask>
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
            if (yamltasks == null)
            {
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
            string yamlraw = "";
            using (TextReader tr = File.OpenText(yamltasks))
                yamlraw = tr.ReadToEnd().Replace("tasks:", "");

            var deserializer = new DeserializerBuilder()
               .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();
            try
            {
                tasks = deserializer.Deserialize<List<BTask>>(yamlraw);
            }
            catch (Exception ex)
            {
                logger.LogError("paths:yamltasks : {o}", yamltasks);
                logger.LogError("GetExecutingAssembly : {o}", Assembly.GetExecutingAssembly().Location);
                throw new Exception($"YamlStream Deserialize Failed: {ex.Message}");
            } 
            return tasks.ToList();
        }
        #endregion 
    }
}
