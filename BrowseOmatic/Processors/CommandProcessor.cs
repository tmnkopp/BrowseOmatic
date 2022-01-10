using BOM.CORE;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BOM
{
    public interface ICommandProcessor
    {
        void Process(BTask task);
    }
    public class CommandProcessor : ICommandProcessor
    {  
        private readonly ISessionContext ctx; 
        private readonly ILogger logger;
        public CommandProcessor(
             ISessionContext ctx, 
             ILogger logger
            )
        { 
            this.ctx = ctx; 
            this.logger = logger;
        }

        public void Process(BTask task)
        {  
            foreach (var taskstep in task.TaskSteps)
            {
                if (taskstep.Cmd.ToLower() == "setwait")
                {
                    ctx.SessionDriver.SetWait(Convert.ToDouble(taskstep.Args[0] ?? "1")); continue;
                }

                var typ = Assm.GetTypes()
                .Where(t => t.Name.Contains(taskstep.Cmd) && typeof(ICommand)
                .IsAssignableFrom(t)).FirstOrDefault();

                Type tCmd = Type.GetType($"{typ.FullName}, {typ.Namespace}") ?? Type.GetType($"{typ.FullName}, BOM");
                ParameterInfo[] PI = tCmd.GetConstructors()[0].GetParameters();
                List<object> oparms = new List<object>();
                int parmcnt = 0; 
                foreach (ParameterInfo parm in PI)
                {
                    string value = null;
                    if (taskstep.Args.Count() >= parmcnt)
                    {
                        value = taskstep.Args[parmcnt];
                        if (value.StartsWith("-p")) value = Prompt(parm);
                    }
                    parmcnt++;
                    if (parm.ParameterType.Name.ToLower().Contains("int")) oparms.Add(Convert.ToInt32(value ?? "0"));
                    else if (parm.ParameterType.Name.ToLower().Contains("bool")) oparms.Add(Convert.ToBoolean(value ?? "false"));
                    else if (parm.ParameterType.Name.ToLower().Contains("double")) oparms.Add(Convert.ToDouble(value ?? "0"));
                    else oparms.Add(value ?? "");
                }
                try
                {
                    ICommand obj = (ICommand)Activator.CreateInstance(tCmd, oparms.ToArray());
                    obj.Execute(ctx);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("ICommand:CreateInstance {c}\n", tCmd);
                    logger.LogWarning("oparms: {o}", JsonConvert.SerializeObject(oparms));
                    logger.LogError("\n{o}", ex.Message);
                }
            }
            
        }
        private static string Prompt(ParameterInfo parm)
        {
            Console.Write($"\n{parm.Name} ({parm.ParameterType.Name}):");
            return Console.ReadLine();
        }
    }
}
