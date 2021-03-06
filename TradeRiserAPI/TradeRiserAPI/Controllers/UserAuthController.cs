using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TradeRiserAPI.Models;
using System.Threading.Tasks;


using System.Web;
using System.Threading;

namespace TradeRiserAPI.Controllers
{
    [RoutePrefix("api/UserAuth")]
    public class UserAuthController : ApiController
    {
        /// <summary>
        /// Register newly obtained token.
        /// After obtain a new authorization token, call this method to register the token
        /// against the username
        /// </summary>
        /// <param name="model">Registeration object contains mulitple fields for registeration</param>
        [Route("Register")]
        [HttpPost]
        public UserInfo RegisterToken(RegisterModel model)
        {
            var dataModel = new DataModel();
            return dataModel.RegisterToken(model.UserName, model.AccessToken);
        }

        /// <summary>
        /// Authenticate the user to use consuming application.
        /// </summary>
        /// <param name="model">An object that contains the username, password and rememberme flag.</param>
        [Route("RequestToken")]
        [HttpPost]
        public string RequestToken(ObtainToken model)
        {
            var restC = new RestClient();
            var resdpon =  restC.GetAccessToken(model.UserName, model.Password, HttpContext.Current.Request);

            var dataModel = new DataModel();
            return resdpon;
            //return dataModel.Login(model.UserName, model.Password, true);
        }


        [Route("GetUserInfoUsername")]
        [HttpPost]
        public UserInfo GetUserInfoUsername(string username)
        {
            var dataModel = new DataModel();
            return dataModel.GetUserInfo_Username(username);
        }


        /// <summary>
        /// Session Token Active.
        /// Call this method to check if Token is still active and valid
        /// If valid it returns true, if not valid or if session has ended it returns
        /// 401 Message - Unauthorised
        /// </summary>
        
        [Route("SessionTokenActive")]
        [HttpGet]
        [ValidationActionFilter]
        public bool SessionTokenActive()
        {
            var context = this.Request.Properties["MS_HttpContext"] as System.Web.HttpContextWrapper;
            var auth = context.Request.Headers["Authorization"];

            return true;
        }

        /// <summary>
        /// Authenticate the user to use consuming application.
        /// </summary>
        /// <param name="model">An object that contains the username, password and rememberme flag.</param>
        [Route("Authenticate")]
        [HttpPost]
        public UserInfo Authenticate(LoginModel model)
        {
            var dataModel = new DataModel();

            return dataModel.Login(model.UserName, model.Password, true);
        }



        // POST api/User/Logout
        [HttpPost]
        [Route("Logout")]
        public bool Logout(UserNameItem username)
        {
            var context = this.Request.Properties["MS_HttpContext"] as System.Web.HttpContextWrapper;
            var auth = context.Request.Headers["Authorization"];

            if (username != null)
            {
                var datamodel = new DataModel();
                string authOnly = auth.Replace("Bearer", "");
                if (datamodel.LogoutToken(username.UserName, authOnly))
                {
                    return true;

                    //return this.Ok(new { message = "Logout successful." });
                }
                else
                {
                    return false;

                    //return this.Ok(new { message = "Logout unsuccessful." });
                }
            }
            return false;
            //return this.Ok(new { message = "Logout unsuccessful." });
        }
    }
}
