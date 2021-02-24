using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using CommandLine;
using CommandLine.Text;
using System.Linq; 
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions; 
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BOM.CORE;
namespace BrowseOmatic
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceProvider serviceProvider = RegisterServices(args);

            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            ILogger logger = serviceProvider.GetService<ILogger<Program>>();
            IAppSettingProvider<Profile> profileProvider = serviceProvider.GetService<IAppSettingProvider<Profile>>();
            IAppSettingProvider<Task> taskProvider = serviceProvider.GetService<IAppSettingProvider<Task>>();

            var exit = Parser.Default.ParseArguments<ExeOptions, TaskOptions>(args)
            .MapResult(
              (IOptions o) => { 
                  return 0;
              },
             (TaskOptions o) => { 
                 return 0;
             },
              errs => 1);
            serviceProvider.Dispose();
        }
        private static ServiceProvider RegisterServices(string[] args)
        {
            var path = Assembly.GetExecutingAssembly().Location.Replace("BOM.dll", "");

            IConfiguration configuration = new ConfigurationBuilder()
                  .SetBasePath(path)
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables()
                  .AddCommandLine(args)
                  .Build();

            var services = new ServiceCollection();
            services.AddLogging(cfg => cfg.AddConsole());
            services.AddSingleton(configuration);
            services.AddTransient<IAppSettingProvider<Profile>, ProfileProvider>();
            services.AddTransient<IAppSettingProvider<Task>, TaskProvider>();
            return services.BuildServiceProvider();
        }
    }
}
