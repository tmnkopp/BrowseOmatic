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
    public class ContextProvider : IAppSettingProvider<SessionContext>
    {
        #region CTOR 
        private readonly IConfiguration configuration;
        private readonly IBScriptParser bomScriptParser;
        private readonly ILogger<ContextProvider> logger;
        public ContextProvider(
            IConfiguration configuration,
            IBScriptParser bomScriptParser,
            ILogger<ContextProvider> logger)
        {
            this.configuration = configuration;
            this.bomScriptParser = bomScriptParser;
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
            var SessionContexts = (from s in sections 
                        let driver = s["conn"].Split("driver:")[1].Split(";")[0] 
                        select new 
                        {
                            name = s["name"], conn = s["conn"], driver = driver 
                        }); 
            List<SessionContext> contexts = new List<SessionContext>();
            foreach (var item in SessionContexts)
            {
                logger.LogInformation("{o}", JsonConvert.SerializeObject(item));
                try
                {
                    var t = Type.GetType(item.driver); 
                    string connectionstring = item.conn;
                    var driver = (ISessionDriver)Activator.CreateInstance(
                        t, new object[] {
                              configuration
                            , bomScriptParser
                            , connectionstring
                        }
                    );
                    contexts.Add(new SessionContext
                    {
                        Name = item.name,
                        SessionDriver = driver
                    });
                    
                } 
                catch (Exception e)
                {
                    logger.LogError("{item}", JsonConvert.SerializeObject(item));
                    logger.LogError("{e}", e.Message);
                    Console.Write($" {e.Message} {e.StackTrace} ");
                    throw e; 
                } 
                Console.WriteLine( $"{item.driver}" ); 
            }  
            return contexts.ToList();
        }
        #endregion
    }

}
 
