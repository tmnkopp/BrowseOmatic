using BOM.CORE;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CS
{ 
    public class CQProcessor : ICommand
    {
        private string _report = null;
        public CQProcessor(string ReportName)
        {
            _report = (string.IsNullOrEmpty(ReportName)) ? "" : ReportName;
        }
        public void Execute(ISessionContext ctx)
        {
            var sd = ctx.SessionDriver;
            sd.Connect();
            sd.Driver.Navigate().GoToUrl("https://localhost/Reports/CustomQueries.aspx");
            ControlPopulate.RadDDL(ctx, "ddl_ReportList", $"{_report}");
            ControlPopulate.RadDDL(ctx, "ddl_Agency", "Justice");
            ControlPopulate.RadDDL(ctx, "ddl_Bureau", 1);
            ControlPopulate.RadDDL(ctx, "ddl_HVA", 1);
            ControlPopulate.RadDDL(ctx, "ddl_Assessment", 1);
            ControlPopulate.RadDDL(ctx, "ddl_POAM", 1);
        }
    }
}
