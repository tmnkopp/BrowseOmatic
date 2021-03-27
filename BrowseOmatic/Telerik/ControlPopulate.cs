using BOM.CORE;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM
{
    public static class ControlPopulate
    {
        public static void RadDDL(ISessionContext ctx, string DDL, string Item)
        {
            ChromeDriver driver = ctx.SessionDriver.Driver;
            var ddl = driver.FindElement(By.CssSelector($"*[id$='{DDL}']"));
            ddl.Click();
            System.Threading.Thread.Sleep(100);
            ddl = driver.FindElement(By.CssSelector($"div[id*='{DDL}_DropDown'] .rddlPopup"));
            var report = ddl.FindElement(By.XPath($"//li[contains(string(), '{Item}')]"))
                ?? ddl.FindElement(By.XPath($"//span[contains(string(), '{Item}')]"));

            report.Click();
        }
        public static void RadDDL(ISessionContext ctx, string DDL, int item = 1)
        {
            ChromeDriver driver = ctx.SessionDriver.Driver;
            var ddl = driver.FindElement(By.CssSelector($"*[id$='{DDL}']"));
            ddl.Click();
            System.Threading.Thread.Sleep(200);
            ddl = driver.FindElement(By.CssSelector($"div[id*='{DDL}_DropDown'] .rddlPopup"));
            var report = ddl.FindElements(By.CssSelector($"li.rddlItem"))[item];
            report.Click();
        }
        public static void GenericForm(ISessionContext ctx)
        {
            Random _random = new Random();
            IList<IWebElement> inputs;
            ChromeDriver driver = ctx.SessionDriver.Driver;
            inputs = driver.FindElements(By.CssSelector("input[id*='Numeric']"));
            foreach (IWebElement input in inputs)
            {
                if (input.GetAttribute("value") == "" && input.GetAttribute("type") != "hidden")
                    input.SendKeys("0");
            }
            inputs = driver.FindElements(By.CssSelector("input[id*='CBPercentage']"));
            foreach (IWebElement input in inputs)
                if (input.GetAttribute("value") == "" && input.GetAttribute("type") != "hidden")
                    input.SendKeys("0");

            inputs = driver.FindElements(By.CssSelector("textarea"));
            foreach (IWebElement input in inputs)
            {
                if (input.Text == "")
                    input.SendKeys(input.GetAttribute("name").Replace("$", ". "));
            }
            inputs = driver.FindElements(By.CssSelector("input[id*='date']"));
            foreach (IWebElement input in inputs)
            {
                if (input.GetAttribute("value") == "" && input.GetAttribute("type") != "hidden")
                    input.SendKeys($"{DateTime.Now.ToShortDateString()}");
            }
            inputs = driver.FindElements(By.CssSelector("input[type='text']"));
            foreach (IWebElement input in inputs)
            {
                if (input.GetAttribute("value") == "" && input.GetAttribute("type") != "hidden")
                {
                    string val = input.TagName;
                    if (input.GetAttribute("class").Contains("data-ipaddress"))
                    {
                        string v = _random.Next(255).ToString();
                        val = $"{v}.{v}.{v}.{v}";
                    }
                    try
                    {
                        input.SendKeys(val);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Source} {e.Message}");
                    }
                }
            }

            inputs = driver.FindElements(By.CssSelector(".RadDropDownList_Default"));
            foreach (IWebElement input in inputs)
            {
                System.Threading.Thread.Sleep(200);
                try
                {
                    input.Click();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Source} {e.Message}");
                    return;
                }
                System.Threading.Thread.Sleep(750);
                IList<IWebElement> rddlItems = driver.FindElements(By.CssSelector($"#{input.GetAttribute("id")}_DropDown .rddlItem"));
                if (rddlItems.Count > 1)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(1, rddlItems.Count - 1);
                    try
                    {
                        rddlItems[index].Click();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"RadDropDownList_Default : {index.ToString()}");
                        Console.WriteLine($"{e.Source} {e.Message}");
                    }
                }
            }

            inputs = driver.FindElements(By.CssSelector(".RadComboBox input[id$='_Input']"));
            foreach (IWebElement input in inputs)
            {
                input.Click();
                var chk = driver.FindElements(By.CssSelector($".rcbList li"))[1]
                    ?? driver.FindElements(By.CssSelector($".rcbList li"))[2]
                    ?? driver.FindElements(By.CssSelector($".rcbList .rcbItem"))[1]
                    ?? driver.FindElements(By.CssSelector($".rcbList .rcbItem"))[2]
                    ;
                try
                {
                    chk.Click();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Source} {e.Message}");
                }
            }
        }
    }
}
