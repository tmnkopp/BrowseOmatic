﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BOM.CORE
{
    public class Url : ICommand
    {
        string url = "";
        public Url(string Url)
        {
            this.url = Url;
        }
        public void Execute(ISessionContext ctx)
        {
            ctx.SessionDriver.GetUrl(this.url).Pause(20);
        }
    }
}
