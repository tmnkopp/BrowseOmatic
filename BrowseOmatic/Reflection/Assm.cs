using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BOM
{
    public static class Assm
    { 
        public static IEnumerable<Type> GetTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assm => assm.GetTypes()).Where(t=>t.IsClass); 
        }  
    }
}
