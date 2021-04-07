using BOM.CORE;
using System;

namespace TelerikCommands
{
    public class PromptCommand : ICommand
    {
        private string prompt = "";
        public PromptCommand(string prompt)
        {
            this.prompt = prompt;
        }
        public void Execute(ISessionContext ctx)
        {
            var sd = ctx.SessionDriver;
            sd.Connect(); 
        }
    }
}
