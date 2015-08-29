using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TradeRiserAPI.Models;
using System.Threading.Tasks;

namespace TradeRiserAPI.Controllers
{
    [RoutePrefix("api/UserAuth")]
    public class UserAuthController : ApiController
    {
        /// <summary>
        /// Register a new user to use the application.
        /// </summary>
        /// <param name="model">Registeration object contains mulitple fields for registeration</param>
        [Route("Register")]
        [HttpPost]
        public bool Register(RegisterModel model)
        {
            var dataModel = new DataModel();
            return dataModel.CreateUserAndAccount(model);
        }


        /// <summary>
        /// Authenticate the user to use consuming application.
        /// </summary>
        /// <param name="model">An object that contains the username, password and rememberme flag.</param>
        [Route("Authenticate")]
        [HttpPost]
        public bool Authenticate(LoginModel model)
        {
            var dataModel = new DataModel();

            return dataModel.Login(model.UserName, model.Password, true);
        }
    }
}
