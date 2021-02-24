
 
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace BOM.CORE
{
    public interface IAutomator
    { 
        void Automate(ISession Session);
    } 
}
