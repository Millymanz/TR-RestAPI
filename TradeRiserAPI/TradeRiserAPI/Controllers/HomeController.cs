using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradeRiserAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "TradeRiser Developers";

            return View();
        }
        
        public ActionResult Documentation()
        {
            ViewBag.Title = "Documentation";

            return View();
        }
    }
}
