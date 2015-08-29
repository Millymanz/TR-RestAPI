using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TradeRiserAPI.Models;
using System.Threading;

namespace TradeRiserAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            TradeUtility.Setup();


            StartQueryServicePolling();
        }

        private void StartQueryServicePolling()
        {
            //Disable for now to prevent IIS issues
            ThreadStart pollerThreadStart = new ThreadStart(Poller.InitialiseClient);
            Thread pollerThread = new Thread(pollerThreadStart);

            pollerThread.Start();
        }
    }
}
