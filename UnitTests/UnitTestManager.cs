using BOM.CORE;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    public static class  Utils{
        public static ISessionContext Context()
        { 
            var config = new UnitTestManager().Configuration;
            var mock = new Mock<ILogger<ContextProvider>>();
            ILogger<ContextProvider> logger = mock.Object; 
            SessionContext ctx = new SessionContext();
            ctx.SessionDriver = new SessionDriver(config, logger );
            ctx.SessionDriver.Connect(ctx.configContext.conn); 
            return ctx;
        }
    }
    public class UnitTestManager 
    {
        private IConfiguration _config;

        public UnitTestManager()
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
                    var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false);
                    _config = builder.Build();
                }

                return _config;
            }
        }
       
    }
}
