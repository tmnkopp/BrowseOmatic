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
        [Option('t', "Task", HelpText ="Executes A Task")]
        public string Task { get; set; }
        [Option('p', "Path",Default="", HelpText = "Sets task file path")]
        public string Path { get; set; }
        [Option('c', "Context", Default = null, HelpText = "Sets Context")]
        public string Context { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
        [Option('k', "KeepAlive", HelpText = "Dispose end of session.", Default = false)]
        public bool KeepAlive { get; set; }
    }
    [Serializable]
    [Verb("exe", HelpText = "Type executor.")]
    public class ExeOptions
    { 
        [Option('p', "Prompt", HelpText = "Prompt for Param")]
        public bool Prompt { get; set; }
        [Option('t', "Type", HelpText = "Execute an ICommand")]
        public string Type { get; set; }
        [Option('a', "Assembly", HelpText = "Sets an Assembly")]
        public string Assembly { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
    }
    [Serializable]
    [Verb("config", HelpText = "Config setter.")]
    public class ConfigOptions
    {
        [Option('p', "Path", HelpText = "Sets task file path", Default="")]
        public string Path { get; set; }
        [Option('t', "Type", HelpText = "Execute an ICommand")]
        public string Type { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
    }
}
