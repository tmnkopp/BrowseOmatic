using BOM.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BOM
{

    public interface ITypeProvider
    { 
        IEnumerable<Type> GetCommands(string Context);
    }
    public class TypeProvider : ITypeProvider
    {  
        public IEnumerable<Type> GetCommands(string Context)  
        { 
                return AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(assm => assm.GetTypes())
                 .Where(
                    t => typeof(ICommand).IsAssignableFrom(t)
                        && (t.GetCustomAttribute<CommandMeta>()?.Context == Context
                        )

                 ).ToList();
          
        }  
        public void WriteExecutingAssembly() { 
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass
                    select t;
            q.ToList().ForEach(t => Console.WriteLine(t.Name));
        }
    }
}
