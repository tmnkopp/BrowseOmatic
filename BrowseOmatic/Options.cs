using BOM;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    public interface IOptions { 
        string Type { get; set; }
        string Alias { get; set; }
        bool Verbose { get; set; }
    }
    [Serializable]
    [Verb("task", HelpText = "Task Runner.")]
    public class TaskOptions {  
        [Option('t', "Task")]
        public string Task { get; set; }
        [Option('a', "Alias")]
        public string Alias { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
    }
    [Serializable]
    [Verb("exe", HelpText = "Task Runner.")]
    public class ExeOptions
    {
        [Option('t', "Type")]
        public string Type { get; set; }
        [Option('a', "Automator")]
        public string Automator { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
    }

}
