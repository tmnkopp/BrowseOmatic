using Microsoft.Extensions.Configuration;
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
        public void SetWait(double Wait);
        public IWebElement Select(string ElementSelector);
        public bool ElementExists(string ElementSelector);
        public IConfiguration config { get; }
        public ILogger Log { get; }
        public double Timeout { get; set;  }
        public ChromeOptions ChromeOptions { get; set; }
        public void Create(); 
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
        private double _timeout = .5; 
        public double Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        } 
        public void SetWait(double Timeout) {
            _timeout = Timeout;
        }
        public IConfiguration config => configuration;
        public ChromeOptions ChromeOptions { get; set; } = new ChromeOptions();
        public ChromeDriver driver;
        public ChromeDriver Driver
        {
            get
            {
                if (driver == null) 
                    Create(); 
                return driver;
            }
        }  
        #region Methods
        public virtual void Create() {
            if (driver == null)
            {
                
                string path = configuration.GetSection("paths")["chromedriver"].Replace("chromedriver.exe", "");
                if (string.IsNullOrEmpty(path))
                {
                    path = Environment.GetEnvironmentVariable("bom", EnvironmentVariableTarget.User).ToLower().Replace("bom.exe", "");
                }
                var chromeDriverService = ChromeDriverService.CreateDefaultService(path);
                chromeDriverService.HideCommandPromptWindow = true;
                chromeDriverService.SuppressInitialDiagnosticInformation = true;

                var chromeOptions = configuration.GetSection("ChromeOptions").Get<string[]>();
                if (chromeOptions != null)
                { 
                    foreach (var option in chromeOptions) 
                        ChromeOptions.AddArgument(option); 
                } 
                driver = new ChromeDriver(chromeDriverService, ChromeOptions);
            } 
        }
         
        public SessionDriver Pause(int Time)
        { 
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
            this.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(this.Timeout);
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

            if(ElementSelector.StartsWith("//"))
            {
                elm = Driver.FindElement(By.XPath($"{ElementSelector}"));
                if (elm == null)
                    Console.WriteLine($"{ElementSelector}: Not Found");
                return elm;
            }
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
 