using BOM.CORE; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using BOM;
namespace UnitTests
{
    [TestClass]
    public class ReflectionTests
    {
        public class MoqCommand : ICommand
        {
            public MoqCommand()
            { 
            }
            public MoqCommand(string C1)
            {
            }
            public void Execute(ISessionContext SessionContext)
            {
                 
            }
        }
        [TestMethod]
        public void ICommand_Provides()
        {
            var ctx = new SessionContext();
            var rad = new RadFormFill();
            //.Where(t => t.Name.Contains("RadForm") && typeof(ICommand).IsAssignableFrom(t))
            var typ = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assm => assm.GetTypes())
                    .Where(t => t.Name.Contains("MoqCommand") && typeof(ICommand).IsAssignableFrom(t))
                    .FirstOrDefault();


            //var typ = typs.FirstOrDefault(); 
            ICommand instance = (ICommand)Activator.CreateInstance(typ); 
            instance.Execute(ctx); 

            Type tCmd = Type.GetType($"{typ.FullName}, {typ.Namespace}");
            ConstructorInfo[] ctorinfo = tCmd.GetConstructors();
            var ctors = from c in ctorinfo select c.GetParameters().Count();
            ParameterInfo[] PI = tCmd.GetConstructors()[0].GetParameters();

            Assert.IsNotNull(instance);

        }

        public void AssemblyTest()
        { 
            StringBuilder sb = new StringBuilder(); 
            Assert.IsNotNull(sb);
        }
    }
} 