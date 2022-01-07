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

            var exit = Parser.Default.ParseArguments<ExeOptions, RunOptions, ConfigOptions>(args)
                .MapResult(
                (RunOptions o) =>
                {
                    logger.LogInformation("RunOptions: {o}", JsonConvert.SerializeObject(o)); 
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

                    StringBuilder sb = new StringBuilder();
                    SetYamlPath(o.Path, configuration);
                    tasks = serviceProvider.GetService<IAppSettingProvider<BTask>>();

                    sb.AppendFormat("\n{0}contexts{0}", new string('-', 9));
                    var contexts = configuration.GetSection("contexts").GetChildren();
                    if (contexts == null)
                        logger.LogWarning("{o}", contexts);
                    else
                        foreach (var context in contexts)
                            sb.AppendFormat("\n{0} \t{2} \n{1}", context["name"], context["conn"], context["root"]);

                    sb.AppendFormat("\n\n{0}vars{0}", new string('-', 9));
                    sb.AppendFormat("\n{0}", JsonConvert.SerializeObject(o));
                    sb.AppendFormat("\nEnVar: {0}", Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User));
                    sb.AppendFormat("\nAssmLoc: {0}", Assembly.GetExecutingAssembly().Location);
                    sb.AppendFormat("\nAppSetting: {0}", $"{Assembly.GetExecutingAssembly().Location}\\appsettings.json");
                     
                    var paths = configuration.GetSection("paths");
                    if (paths == null)
                        logger.LogWarning(" TaskProvider config.GetSection null: {o}", paths); 
                     
                    var yamltasks = configuration.GetSection("paths:yamltasks");
                    if (yamltasks == null)
                        logger.LogWarning(" TaskProvider config.GetSection null: {o}", yamltasks);
                    else 
                        sb.AppendFormat("\npaths:yamltasks : {0}", yamltasks.Value);



                    sb.AppendFormat("\n{0}tasks{0}", new string('-', 9));
                    foreach (var t in tasks.Items)
                        sb.AppendFormat("\n\t{0}", $"bom -k -p {o.Path} -t {t.Name}");

                    logger.LogInformation("{0}", sb.ToString());
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
