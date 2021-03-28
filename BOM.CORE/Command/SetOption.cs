using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    public class SetOption : ICommand
    {
        private string Element = "";
        private int index = 0;
        public SetOption( string Element, int index)
        {
            this.Element = Element;
            this.index = index;
        }

        public void Execute(ISessionContext ctx)
        {
            var dvr = ctx.SessionDriver;
            ctx.SessionDriver.Pause(0);
            SelectElement sections = new SelectElement(dvr.Select(Element));
            if (sections != null)
            {
                try
                {
                    sections.SelectByIndex(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"sections.SelectByIndex index out of range {e.Message}");
                }
            }  
        }
    }
}
