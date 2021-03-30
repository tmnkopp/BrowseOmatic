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
                    if (o.Path.ToString().EndsWith("yaml"))
                    {
                        if (!o.Path.Contains(":\\"))
                            o.Path = Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User).ToLower().Replace("bom.exe", o.Path); 
                        configuration.GetSection("paths:yamltasks").Value = o.Path.ToString();
                        logger.LogInformation("{o}", configuration.GetSection("paths:yamltasks").Value);
                    }

                    ctxs = serviceProvider.GetService<IAppSettingProvider<SessionContext>>();
                    tasks = serviceProvider.GetService<IAppSettingProvider<BTask>>(); 
                     
                    var task = (from t in tasks.Items where t.Name.ToUpper().Contains(o.Task.ToUpper()) select t).FirstOrDefault();
                    ISessionContext ctx = (from c in ctxs.Items where c.Name == task.Context select c).FirstOrDefault();
                    ctx.SessionDriver.Connect();
                    
                    foreach (var taskstep in task.TaskSteps)
                    {
                        if (taskstep.Cmd.ToLower() == "connect")
                        {
                            ctx = (from c in ctxs.Items where c.Name == taskstep.Args[0] select c).FirstOrDefault();
                            ctx.SessionDriver.Connect(); continue;
                        }
                        if (taskstep.Cmd.ToLower() == "setwait")
                        {
                            ctx.SessionDriver.SetWait(Convert.ToInt32(taskstep.Args[0] ?? "500")); continue;
                        } 
                        var typ = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(assm => assm.GetTypes())
                                .Where(t => t.Name.Contains(taskstep.Cmd) && typeof(ICommand).IsAssignableFrom(t))
                                .FirstOrDefault();

                        Type tCmd = Type.GetType($"{typ.FullName}, {typ.Namespace}");
                        if (tCmd == null) tCmd = Type.GetType($"{typ.FullName}, BOM");
                        ParameterInfo[] PI = tCmd.GetConstructors()[0].GetParameters();
                        List<object> oparms = new List<object>();
                        int parmcnt = 0;
                        foreach (ParameterInfo parm in PI)
                        {
                            string value = taskstep.Args[parmcnt];
                            parmcnt++;
                            if (parm.ParameterType.Name.Contains("Int"))
                                oparms.Add(Convert.ToInt32(value));
                            else if (parm.ParameterType.Name.Contains("Bool"))
                                oparms.Add(Convert.ToBoolean(value));     
                            else
                                oparms.Add(value);
                        }
                        ICommand obj = (ICommand)Activator.CreateInstance(tCmd, oparms.ToArray());
                        obj.Execute(ctx);
                    }
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

                    if (o.Path.ToString().EndsWith("yaml"))
                    {
                        if (!o.Path.Contains(":\\")) 
                            o.Path = Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User).ToLower().Replace("bom.exe", o.Path);
                     
                        configuration.GetSection("paths:yamltasks").Value = o.Path.ToString();
                        logger.LogInformation("{o}", configuration.GetSection("paths:yamltasks").Value);
                    }
   
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

        private static ServiceProvider RegisterServices(string[] args)
        {
            var exeassmloc = Assembly.GetExecutingAssembly().Location.ToLower().Replace("bom.dll", "");
            var bomloc = Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User)?.ToLower().Replace("bom.exe", ""); 
            if (exeassmloc.Contains("\\appdata\\") && bomloc != null)
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
            services.AddTransient<IAppSettingProvider<BTask>, YmlTaskProvider>();
            services.AddTransient<ITypeParamProvider, TypeParamProvider>();
            services.AddTransient<ITypeProvider, TypeProvider>();
            services.AddTransient<IBScriptParser, BScriptParser>(); 
            return services.BuildServiceProvider();
        }
    }
}
