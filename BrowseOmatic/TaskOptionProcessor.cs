using BOM.CORE;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOM
{
    public interface OptionProcessor{ 
    
    }
    public class TaskOptionProcessor
    {
        #region CTOR 
        private readonly IConfiguration configuration;
        private readonly IAppSettingProvider<Task> tasks;
        private readonly IAppSettingProvider<SessionContext> ctxs;
        private readonly ILogger<TaskOptionProcessor> logger;
        public TaskOptionProcessor(
            IConfiguration Configuration,
            IAppSettingProvider<Task> Tasks,
            IAppSettingProvider<SessionContext> SessionContexts,
            ILogger<TaskOptionProcessor> Logger)
        {
            configuration = Configuration;
            tasks = Tasks;
            ctxs = SessionContexts;
            logger = Logger;
        }
        #endregion


        public void Process(TaskOptions o, ProcessTask taskProcessor) { 

        }
    }
}
