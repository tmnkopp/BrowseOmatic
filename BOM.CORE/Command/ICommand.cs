using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    public interface ICommand
    {
        public void Execute(ISessionContext SessionContext);
    } 
}
