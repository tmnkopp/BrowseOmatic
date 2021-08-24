﻿using System;
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
using TelerikCommands;
using System.Diagnostics;
 

namespace BOM
{
    class Program
    {
        static void Main(string[] args)
        {
             
            ServiceProvider serviceProvider = RegisterServices(args);
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            ILogger logger = serviceProvider.GetService<ILogger<Program>>();
            IAppSettingProvider<SessionContext> ctxs;
            IAppSettingProvider<BTask> tasks;
            ITypeParamProvider typeParamProvider;
 

            var exit = Parser.Default.ParseArguments<ExeOptions, CommandOptions, ConfigOptions>(args)
                .MapResult(
                (CommandOptions o) =>
                {
                    logger.LogInformation("CommandOptions: {o}", JsonConvert.SerializeObject(o)); 
                    SetYamlPath(o.Path, configuration);
                    ctxs = serviceProvider.GetService<IAppSettingProvider<SessionContext>>();
                    tasks = serviceProvider.GetService<IAppSettingProvider<BTask>>();
                    var task = (from t in tasks.Items where t.Name.ToUpper().Contains(o.Task.ToUpper()) select t).FirstOrDefault();
                    ISessionContext ctx = (from c in ctxs.Items where c.Name == (o.Context ?? task.Context) select c).FirstOrDefault();
                    CommandProcessor processor = new CommandProcessor(ctx, logger);
                    processor.Process(task);
                    if (!o.KeepAlive) ctx.SessionDriver.Dispose();
                    return 0;
                },
                (ExeOptions o) => {

                    ctxs = serviceProvider.GetService<IAppSettingProvider<SessionContext>>(); 
                    typeParamProvider = serviceProvider.GetService<ITypeParamProvider>();
                    logger.LogInformation("{o}", JsonConvert.SerializeObject(o)); 
                     
                    var typ = Assm.GetTypes().Where(t => t.Name.Contains(o.Type)).FirstOrDefault();

                    Type type = Type.GetType($"{typ.FullName}, {typ.Namespace}");
                    if (type==null) type = Type.GetType($"{typ.FullName}, BOM");
                    var oparam = typeParamProvider.Prompt(type);
                    ICommand obj = (ICommand)Activator.CreateInstance(Type.GetType($"{typ.FullName}, {typ.Namespace}"), oparam);

                    var objctx = obj.GetType().GetCustomAttribute<CommandMeta>()?.Context;
                    ISessionContext ctx = (from c in ctxs.Items where c.Name == objctx select c).FirstOrDefault();
                    obj.Execute(ctx);
                    return 0;

                }, (ConfigOptions o) => {

                    logger.LogInformation("{o}", configuration.GetSection("paths:yamltasks").Value);
                    SetYamlPath(o.Path, configuration);
                    tasks = serviceProvider.GetService<IAppSettingProvider<BTask>>(); 
                    logger.LogInformation("{p} tasks: {t}", o.Path,  string.Join(", ", (from t in tasks.Items select t.Name)));
                  
                    logger.LogInformation("{o}", JsonConvert.SerializeObject(o));
                    logger.LogInformation("EnVar {o}", Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User));
                    logger.LogInformation("AssmLoc {o}", Assembly.GetExecutingAssembly().Location);
                    var paths = configuration.GetSection("paths");
                    if (paths == null)
                        logger.LogWarning(" TaskProvider config.GetSection null: {o}", paths);
                    else
                        logger.LogInformation(" TaskProvider paths : {o}", paths.GetChildren().Count().ToString());
                     
                    var yamltasks = configuration.GetSection("paths:yamltasks");
                    if (yamltasks == null)
                        logger.LogWarning(" TaskProvider config.GetSection null: {o}", yamltasks);
                    else
                        logger.LogInformation(" TaskProvider yamltasks : {o}", yamltasks.Value); 

                    return 0;
                },
                errs => 1);
            serviceProvider.Dispose();
        }
        private static void SetYamlPath(string Path, IConfiguration configuration) {
            if (!string.IsNullOrEmpty(Path.ToString()))
            {
                if (!Path.Contains(":\\"))
                    Path = Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User).ToLower().Replace("bom.exe", Path);
                if (!Path.EndsWith(".yaml"))
                    Path += ".yaml";
                configuration.GetSection("paths:yamltasks").Value = Path.ToString(); 
            }
        }
        private static ServiceProvider RegisterServices(string[] args)
        {
            
            try
            {
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                Console.WriteLine($"loadedAssemblies {loadedAssemblies}");
                var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();
                Console.WriteLine($"loadedPaths {loadedPaths}");
                var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Commands.dll");
                Console.WriteLine($"referencedPaths {referencedPaths}");
                var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
             
                toLoad.ForEach(
                    path => loadedAssemblies.Add(
                        AppDomain.CurrentDomain.Load(
                            AssemblyName.GetAssemblyName(path)
                            )
                        )
                );
                Console.WriteLine("Assemblies.Added"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Assembly Load Fail: " + ex.Message); 
            }
            try
            { 
                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Console.Clear(): " + ex.Message); 
            }

            var exeassmloc = Assembly.GetExecutingAssembly().Location.ToLower().Replace("bom.dll", "");
            var bomloc = Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User)?.ToLower().Replace("bom.exe", ""); 
            if (Regex.IsMatch(exeassmloc, @"\\appdata\\") && bomloc != null)
            { 
                try
                {
                    File.Delete($"{exeassmloc}appsettings.json");
                    File.Copy($"{bomloc}appsettings.json", $"{exeassmloc}appsettings.json");
                }
                catch (Exception)
                { 
                    throw;
                } 
            }
             
            IConfiguration configuration = new ConfigurationBuilder()
                  .SetBasePath(exeassmloc)
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables()
                  .AddCommandLine(args)
                  .Build();
             
            var services = new ServiceCollection();
            services.AddLogging(cfg => cfg.AddConsole());
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Program>>());
            services.AddSingleton(configuration);
            services.AddTransient<IAppSettingProvider<SessionContext>, ContextProvider>();
            services.AddTransient<IAppSettingProvider<BTask>, TaskProvider>();
            services.AddTransient<ITypeParamProvider, TypeParamProvider>();
            services.AddTransient<ITypeProvider, TypeProvider>(); 
            return services.BuildServiceProvider();
        }

        private static string Prompt(ParameterInfo parm) { 
            Console.Write($"\n{parm.Name} ({parm.ParameterType.Name}):");
            return Console.ReadLine(); 
        }
    }
}
