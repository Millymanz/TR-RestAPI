using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using TradeRiserAPI.Providers;
using TradeRiserAPI.Models;

using System.Threading.Tasks;
using System.Json;
using System.Web.Mvc;
using System.Web.Http.Controllers;

using System.Net;
using System.Net.Http;
using System.Threading;

using Microsoft.Owin.Security.Infrastructure;

namespace TradeRiserAPI
{
    public class UsernameHolder
    {
        public string Username;
    }

    //public class MessageHandler1 : DelegatingHandler
    //{
    //    protected async override Task<HttpResponseMessage> SendAsync(
    //        HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        //Debug.WriteLine("Process request");
    //        // Call the inner handler.
    //        var response = await base.SendAsync(request, cancellationToken);
    //        //Debug.WriteLine("Process response");
    //        return response;
    //    }
    //}

   

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ValidationActionFilter : System.Web.Http.AuthorizeAttribute
    {
        private string _responseReason = "";
        private bool _runBaseMethod = false;

        private List<string> _apiKeysAllowed = new List<string>();

        public ValidationActionFilter()
        {
            var apiKeys = System.Configuration.ConfigurationManager.AppSettings["APISecret"].ToString().Split(':');
            if (apiKeys != null)
            {
                _apiKeysAllowed = apiKeys.ToList();
            }
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {

            base.OnAuthorization(actionContext); 

        }   

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            if (!string.IsNullOrEmpty(_responseReason))
                actionContext.Response.ReasonPhrase = _responseReason;

            if (_runBaseMethod)
            {
                base.HandleUnauthorizedRequest(actionContext);
                _runBaseMethod = false;
            }
            //if token has expired then run base class method
            // _runBaseMethod = true
            //set this to false afterwards
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var dataModel = new DataModel();

            //Check database using access token
            //Or username or email
            //has the user logged out or is logged in
            bool userLoggedIn = false;
            var content = actionContext.Request.Content.ReadAsStringAsync().Result;


            //actionContext.Request.Content.ReadAsStringAsync().Result = "Dennis";

            var authorization = actionContext.Request.Headers.Authorization;

            try
            {
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                if (json != null)
                {
                    if (json["Username"] != null)
                    {
                        var usernameBasic = (string)json["Username"];
                        var apiKeySent = (string)json["APIKEY"];

                        //chnage this feature to check for API Key
                        //if (LoginCheckProcess(userLoggedIn, dataModel, usernameBasic, authorization.Parameter) == false)
                        
                        if (_apiKeysAllowed.Any(m => m == apiKeySent) == false)
                        {
                            return false;
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                //failed try again

                var usernameContent = content.ToLower();

                if (usernameContent.Contains("username"))
                {
                    var indexEqualSign = usernameContent.IndexOf("=");
                    var emailExtract = usernameContent.Substring(indexEqualSign + 1);
                    var correctedEmail = emailExtract.Replace("%40", "@");

                    if (LoginCheckProcess(userLoggedIn, dataModel, correctedEmail, authorization.Parameter) == false)
                    {
                        return false;
                    }
                }


            }
            //if token has expired and user is logged out then
            //then user must request a new token
            //return false;

            //return base.IsAuthorized(actionContext);
            return true;
        }

        private bool LoginCheckProcess(bool userLoggedIn, DataModel dataModel, string username, string authorization)
        {
            userLoggedIn = dataModel.CheckUserLoginStatus(username, authorization);

            if (userLoggedIn == false)
            {
                _responseReason = "Access Denied - User Currently Logged Out";
                return false;
            }
            //if logged out then return "Access Denied"
            //Access Denied - User Currently Logged Out
            //return false


            //if user is logged in return true

            //if token has expired then run base class method
            // _runBaseMethod = true
            //set this to false

            if (dataModel.HasTokenExpired(username, authorization))
            {
                _runBaseMethod = true;
            }
            return true;
        }
    }


    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static int TokenValidationPeriodInMin = 
            Convert.ToInt32(System.Configuration.
            ConfigurationManager.AppSettings["SessionTime"].ToString());


        public static string PublicClientId { get; private set; }

       
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                //AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(TokenValidationPeriodInMin),
                AllowInsecureHttp = true,                 
          
            };


            // Enable the application to use bearer tokens to authenticate users
            
            
            //disabling the token issue feature
            //app.UseOAuthBearerTokens(OAuthOptions);
            

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}
