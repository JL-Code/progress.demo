using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace progress.demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        /// <summary>
        /// 聊天测试界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Chat()
        {
            return View();
        }

        public ActionResult WindowsSignalrTest()
        {
            return View();
        }

        public async Task<ActionResult> WebConnectionSignalr()
        {
            var manager = new SignalRManager("http://127.0.0.1:8000", "siteUpgradeHub");
            await manager.StartAsync();
            manager.Hello("你好 我是web站点");
            return Content("");
        }
    }
}
