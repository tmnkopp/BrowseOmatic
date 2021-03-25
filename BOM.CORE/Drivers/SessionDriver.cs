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

namespace BOM.CORE
{
    public interface ISessionDriver
    {
        public ChromeDriver Driver { get; }
        public SessionDriver SendKeys(string Element, string Content);
        public SessionDriver Click(string Element);
        public SessionDriver GetUrl(string Url);
        public SessionDriver Pause(int Time);
        public IWebElement Select(string ElementSelector);
        public bool ElementExists(string ElementSelector);
        public string connstr { get; set; }
        public bool Connected { get; set; }
        public ILogger Log{ get;  }
        public void Connect(); 
        public void Dispose(); 
    }
    public class SessionDriver: ISessionDriver
    {
        public string connstr { get; set; }
        private readonly IBScriptParser scriptParser;
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        public ILogger Log => logger; 
        public SessionDriver(
            IConfiguration configuration,
            ILogger logger,
            IBScriptParser scriptParser,
            string ConnectionString  )
        {
            connstr = ConnectionString;
            this.scriptParser = scriptParser;
            this.configuration = configuration;
            this.logger = logger;
        }
        public bool Connected { get; set; }

        public ChromeDriver driver;
        public ChromeDriver Driver
        {
            get
            {
                if (driver == null)
                {
                    ChromeOptions options = new ChromeOptions();
                    var path = configuration.GetSection("paths")["chromedriver"];
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
        public virtual void Connect() {  
            if (!Connected)
            {
                if (string.IsNullOrEmpty(connstr))
                    throw new ArgumentNullException("driver connection string empty"); 
                foreach (var spr in scriptParser.Parse(connstr))
                {  
                    try
                    {
                        if (spr.QualifiedCommand=="GetUrl")  Driver.Navigate().GoToUrl($"{spr.Arguments[0]}");
                        if (spr.QualifiedCommand=="SendKeys") SendKeys(spr.Arguments[0], spr.Arguments[1]); 
                        if (spr.QualifiedCommand=="Click") Click(spr.Arguments[0] );
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Connection failed {0} {1}", connstr, ex.Message); 
                    }
                }
                Connected = true;
            } 
        }
        public SessionDriver Pause(int Time = 500)
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
 