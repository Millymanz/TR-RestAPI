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

        protected void Application_PreSendRequestContent()
        {
            bool bFound = false;
            foreach (var item in Context.Request.Headers.Keys)
            {
                if (item.ToString() == "Authorization")
                {
                    bFound = true;
                }
            }

            HttpResponse response = HttpContext.Current.Response;

            var contentss = response.Output.ToString();


            var encoding = System.Text.ASCIIEncoding.ASCII;

            if (response.OutputStream.CanRead)
            {
                using (var reader = new System.IO.StreamReader(response.OutputStream, encoding))
                {
                    string responseText = reader.ReadToEnd();
                    int tehhfj = 0;
                }
            }

            System.Diagnostics.Debug.WriteLine("PreSendRequestContent");

            // Some code here!
        }

        protected void Application_RequestCompleted()
        {
            System.Diagnostics.Debug.WriteLine("RequestCompleted");

            // Some code here!
        }

        protected void Application_ReleaseRequestState()
        {
            System.Diagnostics.Debug.WriteLine("ReleaseRequestState");
            // Some code here!
        }

        protected void Application_PostAuthorizeRequest()
        {

            System.Diagnostics.Debug.WriteLine("PostAuthorizeRequest");

            // Some code here!
        }

        protected void Application_PostAuthenticateRequest()
        {
            System.Diagnostics.Debug.WriteLine("PostAuthenticateRequest");

            // Some code here!
        }

        protected void Application_PreSendRequestHeaders()
        {
            //bool bFound = false;
            //foreach (var item in Context.Request.Headers.Keys)
            //{
            //    if (item.ToString() == "Authorization")
            //    {
            //        bFound = true;
            //    }
            //}

            //if (bFound)
            //{
            //    var auth = Context.Request.Headers["Authorization"];

            //    Context.Response.StatusCode = 401;
            //    Context.Response.StatusDescription = "Unauthorized: Access is denied due to invalid credentials.";

            //    Context.Response.Write("Do Work");

            //}

            System.Diagnostics.Debug.WriteLine("PreSendRequestHeaders");

            // Some code here!
        }

        private void Session_Start(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Session_Start");
        }

        private void Session_End(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Session_End");
        }


    }
}
