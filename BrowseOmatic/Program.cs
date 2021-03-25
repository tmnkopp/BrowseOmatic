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
using BOM.CS;

namespace BOM
{
    class Program
    {
        static void Main(string[] args)
        { 
            ServiceProvider serviceProvider = RegisterServices(args);

            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            ILogger logger = serviceProvider.GetService<ILogger<Program>>();
            IAppSettingProvider<SessionContext> ctxs = serviceProvider.GetService<IAppSettingProvider<SessionContext>>();
            IAppSettingProvider<BTask> tasks = serviceProvider.GetService<IAppSettingProvider<BTask>>();
            ITypeParamProvider typeParamProvider = serviceProvider.GetService<ITypeParamProvider>();
            ITypeProvider typeProvider = serviceProvider.GetService<ITypeProvider>();
            IBScriptParser bomScriptParser = serviceProvider.GetService<IBScriptParser>();
             
            var exit = Parser.Default.ParseArguments<ExeOptions, CommandOptions>(args)
                .MapResult(
                (CommandOptions o) =>
                { 
                    logger.LogInformation("{o}", JsonConvert.SerializeObject(o));
                    var task = (from t in tasks.Items where t.Name.Contains(o.Task) select t).FirstOrDefault();
                    var ctx = (from c in ctxs.Items where c.Name == task.Context select c).FirstOrDefault();
                    ctx.SessionDriver.Connect();
                    
                    foreach (var taskstep in task.TaskSteps)
                    {
                        var typ = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(assm => assm.GetTypes())
                                .Where(t => t.Name.Contains(taskstep.Cmd) && t.IsClass == true)
                                .FirstOrDefault();

                        Type tCmd = Type.GetType($"{typ.FullName}, {typ.Namespace}"); 
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
                            else if (parm.ParameterType.Name.Contains("IBScriptParser"))
                                oparms.Add(bomScriptParser);
                            else
                                oparms.Add(value);
                        }
                        ICommand obj = (ICommand)Activator.CreateInstance(tCmd, oparms.ToArray());
                        obj.Execute(ctx);
                    }
                    return 0;
                },
                (ExeOptions o) => {

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
