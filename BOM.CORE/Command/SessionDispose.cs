using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{
    public class SessionDispose : ICommand
    {
        private int timeOut = 0;
        public SessionDispose(int TimeOut = 0)
        {
            this.timeOut = TimeOut;
        }
        public void Execute(ISessionContext ctx)
        { 
            ctx.SessionDriver.Pause(timeOut).Dispose();
        }
    }
}
