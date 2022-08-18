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
 
                Type tCmd = (from t in Assm.GetTypes() 
                            where t.Name.Contains(taskstep.Cmd) && typeof(ICommand).IsAssignableFrom(t) 
                            select t ).FirstOrDefault();

                ParameterInfo[] PI = tCmd.GetConstructors()[0].GetParameters();
                List<object> oparms = new List<object>();
                int parmcnt = 0; 
                foreach (ParameterInfo parm in PI)
                {
                    string value = (taskstep.Args.Count() >= parmcnt) ? taskstep.Args[parmcnt] : null; 
                    parmcnt++; 
                    if (parm.ParameterType.Name.ToLower().Contains("int")) oparms.Add(Convert.ToInt32(value ?? "0"));
                    else if (parm.ParameterType.Name.ToLower().Contains("bool")) oparms.Add(Convert.ToBoolean(value ?? "false"));
                    else if (parm.ParameterType.Name.ToLower().Contains("double")) oparms.Add(Convert.ToDouble(value ?? "0"));
                    else oparms.Add(value);
                }
                try
                {
                    ICommand obj = (ICommand)Activator.CreateInstance(tCmd, oparms.ToArray());
                    obj.Execute(ctx);
                }
                catch (Exception ex)
                { 
                    logger.LogError("{@ICommandError}", new{ tCmd, oparms, ex.Message } );
                }
            }
            
        } 
    }
}
