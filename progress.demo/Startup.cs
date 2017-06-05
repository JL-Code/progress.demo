using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace progress.demo
{
    /// <summary>
    /// 启动类
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}