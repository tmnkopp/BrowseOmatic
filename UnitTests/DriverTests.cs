using BOM.CORE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace UnitTests
{
    class Item { 
        public string conn { get; set; }
    }
    [TestClass]
    public class DriverTests
    {
     
        [TestMethod]
        public void Driver_Constructs()
        {
            var _typeof = typeof(SessionDriver);
            var cons = _typeof.GetConstructors();
            Item item = new Item();
            var t = Type.GetType("BOM.CORE.SessionDriver, BOM.CORE");
 
            string connectionstring = item.conn;
            var driver = (ISessionDriver)Activator.CreateInstance(
                t , new object[] { 
                } 
                );
            Assert.IsNotNull(driver);
        }
    }
}
