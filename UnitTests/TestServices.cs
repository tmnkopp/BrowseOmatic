using BOM.CORE;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests
{
    public static class  Session{
        public static ISessionContext Context(string context)
        { 
            var config = new TestServices().Configuration;
            var mock = new Mock<ILogger<ContextProvider>>();
            ILogger<ContextProvider> logger = mock.Object;
            ContextProvider contextProvider = new ContextProvider(config, logger);
            SessionContext ctx = (from c in contextProvider.Items where c.Name == context select c).FirstOrDefault();
            //ctx.SessionDriver.Connect(ctx.configContext.conn);
            return ctx;
        }
    }
    public class TestServices 
    {
        private IConfiguration _config; 
        public TestServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(Configuration); 
        }
        public IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath("c:\\bom\\")
                        .AddJsonFile($"appsettings.json", optional: false);
                    _config = builder.Build();
                }

                return _config;
            }
        } 
    }
}
