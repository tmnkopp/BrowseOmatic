using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BOM.CORE
{
    public interface IProcessor {
        void Execute(ISessionContext ctx);
    }
    public class TaskProcessor : IProcessor
    {
        private BTask task;
        private ILogger logger; 
        public TaskProcessor(BTask task, ILogger Logger)
        {
            this.task = task;
            this.logger = Logger;
        }
        public void Execute(ISessionContext ctx)
        {
            ctx.SessionDriver.Connect();
            logger.LogInformation("{task}", JsonConvert.SerializeObject(task));

            foreach (var step in task.TaskSteps)
            {
                var typ = AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(assm => assm.GetTypes())
                             .Where(t => t.Name.Contains(step.Cmd) && t.IsClass == true)
                             .FirstOrDefault();

                Type tCmd = Type.GetType($"{typ.FullName}, {typ.Namespace}");
                ParameterInfo[] PI = tCmd.GetConstructors()[0].GetParameters();
                List<object> oparms = new List<object>();
                int parmcnt = 0;
                foreach (ParameterInfo parm in PI)
                {
                    string value = step.Args[parmcnt];
                    if (value.Contains("-p"))
                    {
                        Console.Write($"\n{parm.Name} ({parm.ParameterType.Name}):");
                        value = Console.ReadLine();
                    }
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
        }
    }
}
