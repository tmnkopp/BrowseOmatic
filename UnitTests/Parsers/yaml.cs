﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
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
        [TestMethod]
        public void Serialize_Config()
        {
            Task task = new Task();
            task.TaskSteps.Add(new TaskStep("URL", new string[] { "https://localhost/login.aspx" }));
            task.TaskSteps.Add(new TaskStep("Key", new string[] { "UserName", "Bill-D-Robertson" }));
            task.TaskSteps.Add(new TaskStep("Key", new string[] { "Password", "P@ssword1" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] {  "LoginButton" })); 
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "Accept" }));  
            //CommandProcessor processor = new CommandProcessor(Session.Context(task.Context), new Mock<ILogger<ContextProvider>>().Object);
            //processor.Process(task); 
            this.WriteTask(task);
        }
        [TestMethod]
        public void Deserialize_Config()
        {
            var json = File.ReadAllText($@"c:\bom\unittest\a_test.json");
            var result =  JsonSerializer.Deserialize<List<ConfigContext>>(json);

            var task = new Task();
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "111" }));
            task.TaskSteps.Add(new TaskStep("Click", new string[] { "222" }));

            var ts1 = result[0].connectiontask.TaskSteps; 
            var ts2 = task.TaskSteps;
            ts1.AddRange(ts2);

            Assert.IsInstanceOfType(result, typeof(List<ConfigContext>));
        }
        public void WriteTask(Task task)
        { 
            var options = new JsonSerializerOptions { WriteIndented = true };
            string ser = JsonSerializer.Serialize(task, options);
            ser = Regex.Replace(ser, $@"(&o0\r\n\s\s)|(- \*o0\r\n)", "");
            File.WriteAllText($"c:\\bom\\unittest\\a_test.json", ser, Encoding.ASCII);

            var json = File.ReadAllText($@"c:\bom\unittest\a_test.json");
            var Task =
                JsonSerializer.Deserialize<List<ConfigContext>>(json);
        }
    }

    [Serializable]
    public class ConfigContext
    {
        public string name { get; set; }
        public string conn { get; set; }
        public Task connectiontask { get; set; }
        public string root { get; set; }
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
