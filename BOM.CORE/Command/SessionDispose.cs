using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{
    public class SessionDispose : ICommand
    {
        public SessionDispose()
        {
        }
        public void Execute(ISessionContext ctx)
        {
            Console.WriteLine("Session End");
            var response = Console.ReadLine();
            ctx.SessionDriver.Dispose();
        }
    }
}
