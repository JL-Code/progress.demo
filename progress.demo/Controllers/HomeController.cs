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
    }
}
