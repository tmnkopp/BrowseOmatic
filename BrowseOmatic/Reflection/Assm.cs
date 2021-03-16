using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BOM
{
    public static class Assm
    {
        public static string NS = Assembly.GetExecutingAssembly().GetName().Name;
        public static string Path = Assembly.GetExecutingAssembly().Location.Replace("BOM.dll", "");
        public static string AQF(this Type t)
        {
            var assm = Assembly.GetExecutingAssembly();
            return $"{t.FullName}, {t.Namespace}";  
        }  
    }
}
