using BOM;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM
{ 
    [Serializable]
    [Verb("task", HelpText = "Task Runner.")]
    public class TaskOptions {  
        [Option('t', "Task")]
        public string Task { get; set; } 
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
    }
    [Serializable]
    [Verb("cmd", HelpText = "Command Runner.")]
    public class CommandOptions
    {
        [Option('t', "Task")]
        public string Task { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
    }
    [Serializable]
    [Verb("exe", HelpText = "Type executor.")]
    public class ExeOptions
    { 
        [Option('p', "Prompt")]
        public bool Prompt { get; set; }
        [Option('t', "Type")]
        public string Type { get; set; }
        [Option('a', "Assembly")]
        public string Assembly { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
    } 
}
