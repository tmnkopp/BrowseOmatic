using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace UnitTests
{
    public class BomTask
    {
        public BomTask()
        {
            BomTaskSteps = new List<BomTaskStep>();
            BomTasks = new List<BomTask>();
        }
        public string Name { get; set; }
        public string Context { get; set; }
        public List<BomTaskStep> BomTaskSteps { get; set; }
        public List<BomTask> BomTasks { get; set; }
    }
    public class BomTaskStep
    {
        public BomTaskStep(string Cmd)
        {
            this.Cmd = Cmd;
        }
        public string Cmd { get; set; }
    }

    [TestClass]
    public class TaskStepProcessorTests
    {
        public void ProcessTask(BomTask task)
        {
            ProcessTask(task);
            ProcessTaskStep(task.BomTaskSteps);
        }
        public void ProcessTaskStep(List<BomTaskStep> BomTaskSteps) 
        {
            foreach (var ts in BomTaskSteps)
            {
                var obj = ts;
                var Cmd = ts.Cmd;
            }
        }
        [TestMethod]
        public void TaskStepProcessor_Processes() 
        {  
            var yaml = new YamlStream();
            using (TextReader tr = File.OpenText(@"C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic\config.yaml"))
                yaml.Load(tr); 
            var root = (YamlMappingNode)yaml.Documents[0].RootNode;

            BomTask Task1 = new BomTask() { BomTaskSteps = new List<BomTaskStep>() { new BomTaskStep("ts1"), new BomTaskStep("ts2") } };
            BomTask Task2 = new BomTask() { BomTaskSteps = new List<BomTaskStep>() { new BomTaskStep("ts11"), new BomTaskStep("ts22") } };
            BomTask Taska = new BomTask() { BomTaskSteps = new List<BomTaskStep>() { new BomTaskStep("tsa1") } };
            BomTask Taskb = new BomTask() { BomTaskSteps = new List<BomTaskStep>() { new BomTaskStep("tsb1") } };
            Task1.BomTasks.Add(Task2);
            var tasks = new List<BomTask>();
            tasks.Add(Task1);
            tasks.Add(Taska);
            tasks.Add(Taskb);

            foreach (var task in tasks)
            {
                ProcessTask(task);
            }

        }
    }
}
