using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOM.CORE 
{ 
    public class Pause : ICommand
    {
        int time = 0; 
        public Pause(int time)
        {
            this.time = time;  
        }
        public override string ToString()
        {
            return $"Pause: [{this.time}]";
        }
        public void Execute(ISessionContext ctx)
        {
            var driver = ctx.SessionDriver.Driver;
            try
            {
                ctx.SessionDriver.Pause(this.time); 
            }
            catch (Exception ex)
            {
                Console.Write($"Invalid Pause {ex.Message} "); 
            }
           
        }
    }  
}
