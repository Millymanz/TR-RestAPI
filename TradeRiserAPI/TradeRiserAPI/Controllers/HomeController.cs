using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradeRiserAPI.Models;

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

        public ActionResult ServiceStatus()
        {
            ViewBag.Title = "Service Status";

            var serviceStatusModel = new ServiceStatusModel();

            var queryHandler = new QueryHandler();
            if (queryHandler.GetBackEndBasicServiceStatus())
            {
                serviceStatusModel.ServiceStatus = "OK";
            }
            else
            {
                serviceStatusModel.ServiceStatus = "FAILED";
            }
            return View(serviceStatusModel);
        }
    }
}
