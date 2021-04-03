using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BOM.CORE 
{
    public class BScriptParseResult {
        public string Command{ get; set; }
        public string Driver{ get; set; }
        public string QualifiedCommand { get; set; }
        public string[] Arguments{ get; set; }
    }
    public class BScriptParser : IBScriptParser
    {
        public IEnumerable<BScriptParseResult> Parse(string BomScript)
        {
            List<string> args = new List<string>();
            var match = Regex.Match(BomScript + ";", "driver:(.*?);");
            var driver = match?.Groups[1]?.Value;
            BomScript = BomScript.Replace(match.Groups[0].Value, "");
            foreach (string cmd in BomScript.Trim().Split(";").TakeWhile(s => s.Trim().Contains(":")))
            {
                args = new List<string>(); 
                var command = cmd.Split(":")[0].Trim();
                args.AddRange(cmd.Split(":")[1].Trim().Split(","));
                yield return new BScriptParseResult
                {
                    Command = command,
                    Driver = driver ?? "driver:BOM.CORE.SessionDriver, BOM.CORE;",  
                    Arguments = command switch
                    { 
                        string s when s.Contains("http") => new string[] {$"{command}:{args[0]}"},
                        _ => args.ToArray()
                    },
                    QualifiedCommand = command switch
                    {
                        "c" => "Click",
                        "s" => "SendKeys",
                        string s when s.Contains("http") => "GetUrl",
                        _ => ""
                    }
                };
            }
        }
    }
}
