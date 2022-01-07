using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BOM.CORE
{
    public class ContextProvider : IAppSettingsProvider<SessionContext>
    {
        #region CTOR 
        private readonly IConfiguration configuration; 
        private readonly ILogger logger;
        public ContextProvider(
            IConfiguration configuration, 
            ILogger logger)
        {
            this.configuration = configuration; 
            this.logger = logger;
        }
        #endregion 
        #region PROPS
        public IEnumerable<SessionContext> Items
        {
            get { return GetItems(); }
        }
        #endregion
        #region Methods 
        private IEnumerable<SessionContext> GetItems()
        { 
            var sections = configuration.GetSection("contexts").GetChildren().AsEnumerable();
            var ConfigContexts = configuration.GetSection("contexts").Get<List<BomConfigContext>>();
 
            List<SessionContext> contexts = new List<SessionContext>();
            foreach (var context in ConfigContexts)
            {
                logger.LogInformation("{o}", JsonConvert.SerializeObject(context));
                try
                {
                    var t = Type.GetType("BOM.CORE.SessionDriver, BOM.CORE");  
                    var driver = (ISessionDriver)Activator.CreateInstance( t, new object[] { configuration, logger });
                    contexts.Add(new SessionContext
                    {
                        Name = context.name,
                        ContextConfig= context,
                        SessionDriver = driver,
                        configuration = this.configuration
                    }); 
                } 
                catch (Exception e)
                {
                    logger.LogError("{item}", JsonConvert.SerializeObject(context));
                    logger.LogError("{e}", e.Message);
                    Console.Write($" {e.Message} {e.StackTrace} ");
                    throw e; 
                }  
            }  
            return contexts.ToList();
        }
        #endregion
    }

}
 
