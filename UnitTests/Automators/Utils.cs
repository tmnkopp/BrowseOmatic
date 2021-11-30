using BOM.CORE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnitTests 
{
    public static class Utils
    {
        public static void WriteTasks(List<BTask> tasks)
        {
            Dictionary<string, List<BTask>> serdict = new Dictionary<string, List<BTask>>();
            serdict.Add("tasks", tasks);
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var ser = serializer.Serialize(serdict);
            ser = Regex.Replace(ser, @"(&o0\r\n\s\s)|(- \*o0\r\n)", "");
            File.WriteAllText($"c:\\bom\\unittest\\{tasks[0].Context}_{tasks[0].Name}.yaml", ser, Encoding.ASCII);
            File.WriteAllText($"c:\\bom\\unittest\\{tasks[0].Context}_{tasks[0].Name}.bat", $"bom run -t {tasks[0].Name} -k -p c:\\bom\\unittest\\{tasks[0].Context}_{tasks[0].Name}.yaml", Encoding.ASCII);
        }
    }
}
