﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BOM.CORE
{
    public interface ISessionDriver
    {
        public ChromeDriver Driver { get; }
        public SessionDriver SendKeys(string Element, string Content);
        public SessionDriver Click(string Element);
        public SessionDriver GetUrl(string Url);
        public SessionDriver Pause(int Time);
        public void SetWait(int Wait);
        public IWebElement Select(string ElementSelector);
        public bool ElementExists(string ElementSelector);
        public IConfiguration config { get; }
        public ILogger Log { get; } 
        public void Connect(string ConnectionString); 
        public void Dispose(); 
    }
    public class SessionDriver: ISessionDriver
    { 
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        public ILogger Log => logger; 
        public SessionDriver(
            IConfiguration configuration,
            ILogger logger  )
        { 
            this.configuration = configuration;
            this.logger = logger;
        } 
        private int timeout = 0;
        public void SetWait(int Timeout) {
            timeout = Timeout;
        }
        public IConfiguration config => configuration;
        public ChromeDriver driver;
        public ChromeDriver Driver
        {
            get
            {
                if (driver == null)
                {
                    ChromeOptions options = new ChromeOptions();
                    var path = configuration.GetSection("paths")["chromedriver"].Replace("chromedriver.exe", ""); 
                    var chromeDriverService = ChromeDriverService.CreateDefaultService(path);
                    chromeDriverService.HideCommandPromptWindow = true;
                    chromeDriverService.SuppressInitialDiagnosticInformation = true;
                    options.AddArgument("log-level=3");
                    driver = new ChromeDriver(chromeDriverService, options); 
                }
                return driver;
            }
        }

        
        #region Methods
        public virtual void Connect(string ConnectionString) {   
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ArgumentNullException("driver connection string empty");
                 
            List<string> args = new List<string>();
            var match = Regex.Match(ConnectionString + ";", "driver:(.*?);"); 
            ConnectionString = ConnectionString.Replace(match.Groups[0].Value, "");
            foreach (string cmd in ConnectionString.Trim().Split(";").TakeWhile(s => s.Trim().Contains(":")))
            {
                args = new List<string>();
                var command = cmd.Split(":")[0].Trim();
                args.AddRange(cmd.Split(":")[1].Trim().Split(",")); 
                try
                {
                    logger.LogInformation("connect {0} [{1}]", command, string.Join(",",args));
                    if (command.Contains("http")) GetUrl($"{command}:{args[0]}");
                    if (command == "s") SendKeys(args[0], args[1]); 
                    if (command == "c") Click(args[0]);
                }
                catch (Exception ex)
                {
                    logger.LogError("Connection failed {0} {1}", ConnectionString, ex.Message); 
                } 
            }   
        }
         
        public SessionDriver Pause(int Time)
        {
            if (Time < 5) 
                Time = timeout; 
            System.Threading.Thread.Sleep(Time);
            return this;
        }
        public SessionDriver SendKeys(string ElementSelector, string Content)
        {
            IWebElement element = Select(ElementSelector);
            if (element != null) element.SendKeys(Content);
            return this;
        }
        public SessionDriver Click(string ElementSelector)
        {
            IWebElement element = Select(ElementSelector);
            if (element != null) element.Click();
            return this;
        }
        public SessionDriver GetUrl(string URL)
        {
            Driver.Navigate().GoToUrl($"{URL}");
            return this;
        }
        public virtual void Dispose()
        {
            Driver.Quit();
        }
        public IWebElement Select(string ElementSelector)
        { 
            IWebElement elm = null; 
            string[] selects = new string[] {
                $"*[id$='{ElementSelector}']",
                $"*[id*='{ElementSelector}']",
                $"*[class$='{ElementSelector}']",
                $"*[class*='{ElementSelector}']",
                $"*[name$='{ElementSelector}']",
                $"*[name*='{ElementSelector}']",
                $"*[type$='{ElementSelector}']",
                $"*[type*='{ElementSelector}']" 
            };
            if (ElementSelector.Contains("[")) {
                selects = new string[] { $"{ElementSelector}" };
            }
            foreach (var select in selects)
                if (ElementExists(select))
                {
                    elm = Driver.FindElement(By.CssSelector($"{select}"));
                    break;
                }
            if (elm == null) 
                Console.WriteLine($"{ElementSelector}: Not Found"); 
            return elm;
        }
        public bool ElementExists(string ElementSelector) {
            var e = Driver.FindElements(By.CssSelector($"{ElementSelector}"));
            if (e?.Count > 0)
                return true;
            else
                return false; 
        }
        #endregion
    } 
} 
 