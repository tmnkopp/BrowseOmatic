using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using BOM.CORE;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class RegexTests
    {
        [TestMethod]
        public void NaiveInputDefaultsDefaults()
        {
            var config = new UnitTestManager().Configuration;
            var match = false;
            var inputs = config.GetSection("NaiveInputDefaults"); 
            foreach (var item in inputs.GetChildren())
            {
                match = Regex.Match("postalcode", item.Key).Success;
            }
 
            Assert.IsTrue(match);

        }
        [TestMethod]
        public void TestMethod1()
        {
            var input = "driver:BOM.CORE.SessionDriver, BOM.CORE;s:username,tim.kopp;s:password,passexpect;c:submit;https://dayman.cyber-balance.com/jira/login.jsp;";
            string driver = Regex.Match(input, "driver:(.*?);").Groups[1].Value;
            // actual = Regex.Replace(input, "driver:(.*?);", "ASDF$1ASDF") ;
            Assert.AreEqual("BOM.CORE.SessionDriver, BOM.CORE", driver);

        } 
    }
}
