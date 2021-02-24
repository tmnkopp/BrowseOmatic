using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    public interface ITask {
        List<TaskStep> TaskSteps { get; set; } 
    }
    public class Task: ITask
    {
        public string Name { get; set; } 
        public List<TaskStep> TaskSteps { get; set; }
    }
    public class TaskStep
    {
        public string cmd { get; set; }
        public string[] args { get; set; }
    }
}
