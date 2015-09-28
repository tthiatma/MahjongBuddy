using MahjongBuddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MahjongBuddy.Controllers
{

    public class HomeController : AppController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
    }
}
