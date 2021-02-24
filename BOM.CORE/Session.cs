using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    public interface ISession
    {
        public Profile Profile { get; set; }
        public IDriver Driver { get; set; }
    }
    public class Session
    {
        public Profile Profile{ get; set; }
        public IDriver Driver { get; set; }
    }
}
