using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BOM.CORE
{

    public class ConfigTaskProvider: IAppSettingProvider<BTask>
    {
        #region CTOR
        private readonly IConfiguration configuration;
        private readonly ILogger<ConfigTaskProvider> logger;
        public ConfigTaskProvider(
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
            var tasksection = configuration.GetSection("tasks");
            if (tasksection == null) 
                logger.LogError("config.GetSection {o}", tasksection); 
 
            return tasksection.GetChildren()
                 .Select(
                    cs => new BTask
                    {
                        Name = cs["name"],  
                        Context = cs["context"], 
                        TaskSteps = cs.GetSection("steps").GetChildren().Select(
                            ts => new TaskStep()
                            {
                                Cmd = ts["cmd"],
                                Args = ts.GetSection("args").GetChildren().Select(s => s.Value).ToArray()
                            }
                            ).ToList()
                    }
                 ).ToList();
        }
        #endregion

    }
}
