using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{
    [Serializable]
    public class BomConfigContext
    {
        public string name { get; set; }
        public string conn { get; set; }
        public BTask conntask { get; set; }
        public string root { get; set; } 
    }
}
