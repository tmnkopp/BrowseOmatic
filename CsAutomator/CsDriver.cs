using BOM.CORE;
using Microsoft.Extensions.Logging;
using System;

namespace CsAutomator
{
    public class CsDriver : BaseSessionDriver, ISessionDriver
    {
        public CsDriver(
              string connectionstring )
            : base(connectionstring )
        {
        }
        public void Connect()
        { 
            if (!Connected)
            { 
                if (string.IsNullOrEmpty(conn))
                    throw new ArgumentNullException("driver connectionstring empty");
                if (conn.Split(";").Length < 2)
                    throw new ArgumentNullException("driver connectionstring invalid");

                var user = conn.Split(";")[1];
                var pass = conn.Split(";")[2];
                GetUrl("https://localhost/")
                    .SendKeys("UserName", user)
                    .SendKeys("Password", pass)
                    .Click("LoginButton").Click("btn_Accept");
                Connected = true;

            }
        }
    }
}
