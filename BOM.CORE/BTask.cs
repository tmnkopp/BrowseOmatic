using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    public interface ITask {
        List<TaskStep> TaskSteps { get; set; } 
        public string Context { get; set; } 
    }
    [Serializable]
    public class BTask: ITask
    {
        public BTask()
        {
            TaskSteps = new List<TaskStep>();
        }
        public BTask(string Name, string Context)
        {
            this.Name= Name;
            this.Context = Context;
            TaskSteps = new List<TaskStep>();
        }
        public string Name { get; set; } 
        public string Context { get; set; } 
        public List<TaskStep> TaskSteps { get; set; }
    }
    public class TaskStep
    {
        public TaskStep()
        { 
        }
        public TaskStep(string Cmd, string[] Args)
        {
            this.Cmd = Cmd;
            this.Args = Args;
        }
        public string Cmd { get; set; }
        public string[] Args { get; set; }
    }
}
