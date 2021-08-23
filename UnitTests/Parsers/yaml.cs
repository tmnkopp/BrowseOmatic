using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting; 
using Newtonsoft.Json;
using System.Text.RegularExpressions; 
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace CoreTests
{ 
    [TestClass]
    public class YamlTests
    { 
        [TestMethod]
        public void Yaml_Serializes()
        { 
            var yml = @"    
  - name: list
    context: jira
    taskSteps: 
    - { cmd: Url, args: [ '']}
  - name: time
    context: jira
    taskSteps: 
    - { cmd: Url, args: ['']} 
"; 
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)   
                .Build(); 
            //yml contains a string containing your YAML
            var t = deserializer.Deserialize<List<Task>>(yml);
            var steps = t[0].TaskSteps;
        }
        [TestMethod]
        public void Yaml_DeSerializes()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 4; i++) {
                Task task = new Task();
                task.Context = "context";
                task.Name = "name";
                task.TaskSteps.Add(new TaskStep("a", new string[] { "1", "2" }));
                task.TaskSteps.Add(new TaskStep("b", new string[] { "11", "22" }));
                task.TaskSteps.Add(new TaskStep("c", new string[] { "111", "222" }));
                tasks.Add(task);
            } 
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(tasks);
            System.Console.WriteLine(yaml);
        }
    }
    [Serializable]
    public class Task 
    {
        public Task()
        {
            TaskSteps = new List<TaskStep>();
        }
        public string Name { get; set; }
        public string Context { get; set; } 
        public List<TaskStep> TaskSteps { get; set; }
    }
    [Serializable]
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
