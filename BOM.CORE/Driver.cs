using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BOM.CORE
{
    public interface IDriver
    {
        public ChromeDriver Driver { get; }
        public BaseDriver SendKeys(string Element, string Content);
        public BaseDriver Click(string Element);
        public BaseDriver Pause(int Time);
        public BaseDriver Connect(Profile profile);
        public bool Connected { get; set; }
    }
    public abstract class BaseDriver
    {
        public ChromeDriver driver;
        public ChromeDriver Driver
        {
            get
            {
                if (driver == null)
                {
                    ChromeOptions options = new ChromeOptions();
                    var chromeDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                    chromeDriverService.HideCommandPromptWindow = true;
                    chromeDriverService.SuppressInitialDiagnosticInformation = true;
                    options.AddArgument("log-level=3");
                    driver = new ChromeDriver(chromeDriverService, options);
                }
                return driver;
            }
        }
        #region Methods
        public BaseDriver Pause(int Time = 500)
        {
            System.Threading.Thread.Sleep(Time);
            return this;
        }
        public BaseDriver SendKeys(string ElementSelector, string Content)
        {
            IWebElement element = Select(ElementSelector);
            if (element != null) element.SendKeys(Content);
            return this;
        }
        public BaseDriver Click(string ElementSelector)
        {
            IWebElement element = Select(ElementSelector);
            if (element != null) element.Click();
            return this;
        }
        public BaseDriver GetUrl(string URL)
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
            IWebElement element = null;
            try
            {
                element = Driver.FindElement(By.CssSelector($"*[id$='{ElementSelector}']")) ??
                    Driver.FindElement(By.CssSelector($"*[id*='{ElementSelector}']")) ??
                    Driver.FindElement(By.CssSelector($"*[class*='{ElementSelector}']")) ??
                    Driver.FindElement(By.CssSelector($"{ElementSelector}"));
            }
            catch (Exception)
            {
                Console.Write($"null: {ElementSelector}");
            }
            return element;
        }
        #endregion 
    }
}
