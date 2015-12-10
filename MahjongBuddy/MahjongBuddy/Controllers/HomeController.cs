using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MahjongBuddy.Controllers
{
    [AllowAnonymous]
    public class HomeController : AppController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "MahjongBuddy description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "MahjongBuddy contact page.";

            return View();
        }
    }
}
