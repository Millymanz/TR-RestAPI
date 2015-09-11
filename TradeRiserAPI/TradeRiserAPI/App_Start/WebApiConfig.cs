using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using TradeRiserAPI.Models;

using System.Web.Cors;
using System.Web.Routing;

namespace TradeRiserAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new ValidationActionFilter());
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //config.MessageHandlers.Add(new MessageHandler1());

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            RouteTable.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            ).RouteHandler = new SessionRouteHandler();  



            // New code
            var cor = System.Configuration.ConfigurationManager.AppSettings["CrossDomainSetting"].ToString();

            var corsAttr = new System.Web.Http.Cors.EnableCorsAttribute(cor, "*", "*");
            config.EnableCors(corsAttr);


            // WebApi Configuration to hook up formatters and message handlers
            // optional
            //RegisterApis(GlobalConfiguration.Configuration);

        }

        public class SessionRouteHandler : System.Web.Routing.IRouteHandler
        {
            System.Web.IHttpHandler System.Web.Routing.IRouteHandler.GetHttpHandler(System.Web.Routing.RequestContext requestContext)
            {
                return new SessionControllerHandler(requestContext.RouteData);
            }
        }

        public class SessionControllerHandler : System.Web.Http.WebHost.HttpControllerHandler, System.Web.SessionState.IRequiresSessionState
        {
            public SessionControllerHandler(System.Web.Routing.RouteData routeData)
                : base(routeData)
            { }
        }

        //public static void RegisterApis(HttpConfiguration config)
        //{
        //    // Add JavaScriptSerializer  formatter instead - add at top to make default
        //    config.Formatters.Insert(0, new JavaScriptSerializerFormatter());
        //}
    }


}
