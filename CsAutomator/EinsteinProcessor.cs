using BOM.CORE;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsAutomator
{
    [ProcessorMeta(Context: "csagency")]
    public class EinsteinProcess : IProcessor
    {
        private int from, to;
        public EinsteinProcess(int From, int To)
        {
            this.from = From;
            this.to = To;
        }
        public void Perform(ISessionDriver SessionDriver)
        {
            SessionDriver.Pause(1000).Pause(1);
        }
    }
}
