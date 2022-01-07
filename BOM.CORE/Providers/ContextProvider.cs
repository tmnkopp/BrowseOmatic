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
    public class ContextProvider : ISettingProvider<SessionContext>
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

        #endregion
        #region Methods 
        public SessionContext Get(string ItemName)
        {  
            var ConfigContexts = configuration.GetSection("contexts").Get<List<BomConfigContext>>(); 
            var ctx = (from c in ConfigContexts where c.name.ToLower()== ItemName.ToLower() select c).FirstOrDefault();
            SessionContext sc;
            try
            {
                var t = Type.GetType("BOM.CORE.SessionDriver, BOM.CORE");
                var driver = (ISessionDriver)Activator.CreateInstance(t, new object[] { configuration, logger });

                sc = new SessionContext
                {
                    Name = ctx.name,
                    ContextConfig = ctx,
                    SessionDriver = driver,
                    configuration = this.configuration
                };
            }
            catch (Exception e)
            {
                logger.LogError("{item}", JsonConvert.SerializeObject(ctx));
                logger.LogError("{e}", e.Message);
                Console.Write($" {e.Message} {e.StackTrace} ");
                throw e;
            }
            return sc;
        }
        #endregion
    }

}
 
