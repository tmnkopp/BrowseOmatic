using BOM.CORE;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TelerikCommands;

namespace UnitTests
{ 
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void PatternSeuqence_Resolves()
        {
            string val = "[X|Y|Z]xxx[1|2|3]xxx[A|B|C]";
            if (Regex.IsMatch(val, @"\[([^\]]+|.+)\]"))
            {
                System.Text.RegularExpressions.Match match = Regex.Match(val, @"\[([^\]]+|.+)\]");
                while (match.Success)
                {
                    var g0 = match.Groups[0];
                    var g1 = match.Groups[1];  
                    int postbegin = g0.Index + g0.Length;
                    int len = val.Length - postbegin;
                    var vals = g1.Value.Split("|");
                    int index = RandomNumberGenerator.GetInt32(0, vals.Length-1);
                    val = $"{val.Substring(0, g0.Index)}{vals[index]}{val.Substring(postbegin, len)}";
                    match = Regex.Match(val, @"\[([^\]]+|.+)\]");
                }
            }
             
            var list = val;
            Assert.IsNotNull(val);
        }

        [TestMethod]
        public void PatternConfig_Resolves()
        {
            string val = "11[1-2].22[1-5].22[1-5].22[1-5]";
            if (Regex.IsMatch(val, @"\[(\d)-(\d)\]"))
            {
                System.Text.RegularExpressions.Match match = Regex.Match(val, @"\[(\d)-(\d)\]");
                while (match.Success)
                {
                    var g0 = match.Groups[0];
                    var g1 = match.Groups[1];
                    var g2 = match.Groups[2];
                    int rInt = RandomNumberGenerator.GetInt32(Convert.ToInt32(g1.Value), Convert.ToInt32(g2.Value));
                    int postbegin = g0.Index + g0.Length;
                    int len = val.Length - postbegin;
                    val = $"{val.Substring(0, g0.Index)}{rInt}{val.Substring(postbegin, len)}";
                    match = Regex.Match(val, @"\[(\d)-(\d)\]");
                }
            }


            var ip = val; 
            Assert.IsNotNull(val);
        }

        [TestMethod]
        public void valueConfig_Resolves()
        {
            string val = "\\dx\\dxxx\\dxx";
            StringBuilder sb = new StringBuilder();
            var vals = val.Split(new string[] { "\\d" }, StringSplitOptions.None);
            for (int i = 1; i < vals.Length; i++)
            {
                sb.Append($"9{vals[i]}");
            } 
            var st = $"{vals[0]}{sb.ToString()}";
            Assert.IsNotNull(st);
        }
          
    }
}
