using BOM;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM
{ 
 
    [Serializable]
    [Verb("run", HelpText = "Task Runner.")]
    public class RunOptions
    {
        [Option('t', "Task", HelpText ="Executes Task")]
        public string Task { get; set; }
        [Option('p', "Path",Default="", HelpText = "Set task file path")]
        public string Path { get; set; }
        [Option('c', "Context", Default = null, HelpText = "Set Context")]
        public string Context { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
        [Option('k', "KeepAlive", HelpText = "Dispose end of session.", Default = false)]
        public bool KeepAlive { get; set; }
        [Option('h', "Headless", HelpText = "Headless session.", Default = false)]
        public bool Headless { get; set; }
    } 
    [Serializable]
    [Verb("config", HelpText = "Config setter.")]
    public class ConfigOptions
    {
        [Option('p', "Path", HelpText = "Sets task file path", Default="")]
        public string Path { get; set; }
        [Option('t', "Task", HelpText = "ICommand")]
        public string Task { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
    }
}
