using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BOM.CORE
{  
    public class TaskProcessor : ICommand
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
            var d = ctx.SessionDriver;
            d.Connect();
            logger.LogInformation("{task}", JsonConvert.SerializeObject(task));
            logger.LogInformation("{SessionDriver}", d );
            foreach (var step in task.TaskSteps)
            {
                MethodInfo methodInfo = d.GetType().GetMethod(step.Cmd);
                int parmcnt = 0;
                List<object> oparms = new List<object>();
                foreach (ParameterInfo parm in methodInfo.GetParameters())
                {
                    string value = step.Args[parmcnt];
                    parmcnt++;
                    if (value.Contains("-p"))
                    { 
                        Console.Write($"\n{parm.Name} ({parm.ParameterType.Name}):");
                        value = Console.ReadLine();
                    }
                    if (parm.ParameterType.Name.Contains("Int"))
                        oparms.Add(Convert.ToInt32(value));
                    else if (parm.ParameterType.Name.Contains("Bool"))
                        oparms.Add(Convert.ToBoolean(value));
                    else
                        oparms.Add(value); 
                }
                var result = methodInfo.Invoke(d, oparms.ToArray()); 
            }
        }
    }
}
