using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE 
{ 
    public class Url : ICommand
    {
        string url = "";
        public Url(string Url)
        {
            this.url = Url;
        }
        public void Execute(ISessionContext ctx)
        {
            ctx.SessionDriver.GetUrl(this.url).Pause(20);
        }
    } 
    public class Click : ICommand
    {
        string element = ""; 
        public Click(string Element)
        {
            this.element = Element; 
        }
        public void Execute(ISessionContext ctx)
        {
            foreach (string ele in element.Split(","))
            {
                ctx.SessionDriver.Click(ele).Pause(75);
            } 
        }
    }
    public class Key : ICommand
    {
        string element = "";
        string content = "";
        public Key(string Element, string Content)
        {
            this.element = Element;
            this.content = Content;
        }
        public void Execute(ISessionContext ctx)
        {
            ctx.SessionDriver.SendKeys(this.element, this.content).Pause(20);
        }
    } 
}
