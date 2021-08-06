using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace SeleniumTests
{ 
    [TestFixture]
    public class SuccessTests
    {
        IWebDriver _driver;
        [SetUp]
        public void Init()
        { 
            var ctx = Session.Context("jira");
            _driver = ctx.SessionDriver.Driver;
        } 
        [Test]
        public void Add()
        {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            _driver.Navigate().GoToUrl("https://dayman.cyber-balance.com/jira/browse/CS-8354");
            _driver.FindElement(By.CssSelector("a[id='opsbar-operations_more']")).Click();
            _driver.FindElement(By.CssSelector("*[id='log-work']")).Click();
            _driver.FindElement(By.CssSelector("input[id='log-work-time-logged']")).SendKeys("5m");
            _driver.FindElement(By.CssSelector("input[id='log-work-submit']")).Click(); 
        }

        [TearDown]
        public void Cleanup()
        {
            _driver.Close();
        }
    } 
}