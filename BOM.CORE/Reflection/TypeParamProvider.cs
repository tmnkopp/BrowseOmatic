using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BOM.CORE
{

    public interface ITypeParamProvider
    {
        object[] Prompt(Type t); 
    }
    public class TypeParamProvider : ITypeParamProvider
    { 
        public object[] Prompt(Type t)
        {
            ParameterInfo[] PI = t.GetConstructors()[0].GetParameters();
            List<object> oparms = new List<object>();
            foreach (ParameterInfo parm in PI)
            {
                Console.Write($"{parm.Name} ({parm.ParameterType.Name}):");
                var item = Console.ReadLine();
                if (parm.ParameterType.Name.Contains("Int"))
                    oparms.Add(Convert.ToInt32(item));
                else if (parm.ParameterType.Name.Contains("Bool"))
                    oparms.Add(Convert.ToBoolean(item));
                else if (parm.ParameterType.Name.Contains("String"))
                    oparms.Add(Convert.ToString(item));
                else
                    oparms.Add(item);
            }
            return oparms.ToArray();
        } 
    }
}
