using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BOM.CORE
{
    public interface IProcessor { 
        public void Perform(ISession Session);
    }
    public class TaskProcessor : IProcessor
    {
        private Task task;
        public TaskProcessor(Task task)
        {
            this.task = task;
        }
        public void Perform(ISession Session)
        { 
            foreach (var step in task.TaskSteps)
            {
                List<object> oparms = new List<object>();
                MethodInfo methodInfo = Session.Driver.GetType().GetMethod(step.cmd);
                foreach (string item in step.args)
                {
                    foreach (ParameterInfo parm in methodInfo.GetParameters())
                    {
                        if (parm.ParameterType.Name.Contains("Int"))
                            oparms.Add(Convert.ToInt32(item));
                        else if (parm.ParameterType.Name.Contains("Bool"))
                            oparms.Add(Convert.ToBoolean(item));
                        else
                            oparms.Add(item);
                    }
                }
                var result = methodInfo.Invoke(Session.Driver, oparms.ToArray());
            }
        }
    }
}
