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

    [ValidationActionFilter]
    [RoutePrefix("api/Query")]
    public class QueryController : ApiController
    {
        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}

        [Route("GetAllEmergingPatternDefaults")]
        [HttpGet]
        public PresentationRenderer GetAllEmergingPatternDefaults()
        {
            PresentationRenderer presentRender = null;

            QueryHandler queryHandler = new QueryHandler();

            var answerTransferPackage = queryHandler.GetAllEmergingPatternDefaults();

            if (answerTransferPackage == null) return null;

            presentRender = new PresentationRenderer(answerTransferPackage);

            return presentRender;
        }

        [Route("GetAllCompletedPatternDefaults")]
        [HttpGet]
        public PresentationRenderer GetAllCompletedPatternDefaults()
        {
            PresentationRenderer presentRender = null;

            QueryHandler queryHandler = new QueryHandler();

            var answerTransferPackage = queryHandler.GetAllCompletedPatternDefaults();

            if (answerTransferPackage == null) return null;

            presentRender = new PresentationRenderer(answerTransferPackage);

            return presentRender;

        }


        /// <summary>
        /// Used for querying the system, return response represents query answer. The result set/ answer object size is managed.
        /// </summary>
        /// <param name="queryItem">Simple object consisting of query string.</param>
        [Route("GetAnswer")]
        [HttpPost]
        public PresentationRenderer GetAnswer(SearchQuery queryItem)
        {
            return Answer(queryItem, true);
        }

        /// <summary>
        /// Used for querying the system, return response represents query answer. Answer object comes as a heavy object, data size is not managed.
        /// </summary>
        /// <param name="queryItem">Simple object consisting of query string.</param>
        [Route("GetAnswerNonManaged")]
        [HttpPost]
        public PresentationRenderer GetAnswerNonManaged(SearchQuery queryItem)
        {
            return Answer(queryItem, false);
        }


        /// <summary>
        /// Used for retrieving symbol data for features like the ChartPad
        /// </summary>
        /// <param name="symbolData">Object containg the symbol and timeframe.</param>
        //[Route("GetSymbolData")]
        //[HttpPost]
        //public QueryService.ChartDataPairing GetSymbolData(SymbolData symbolData)
        //{
        //    QueryHandler queryHandler = new QueryHandler();
        //    return queryHandler.GetSymbolData(symbolData);
        //}

        /// <summary>
        /// Call this to fetch the other results
        /// </summary>
        /// <param name="symbolData">Object containg the symbol and timeframe.</param>
        [Route("GetDataResult")]
        [HttpPost]
        public PresentationRenderer GetDataResult(DataResultRetriever dataResultRetriever)
        {
            QueryHandler queryHandler = new QueryHandler();

            var ans = queryHandler.GetDataResult(dataResultRetriever.QueryId, dataResultRetriever.SelectingSymbol);

            return new PresentationRenderer(ans);
        }

        /// <summary>
        /// Used for retrieving symbol data for features like the ChartPad
        /// </summary>
        /// <param name="symbolData">Object containg the symbol and timeframe.</param>
        [Route("GetSymbolData")]
        [HttpPost]
        public double[][] GetSymbolData(SymbolData symbolData)
        {
            QueryHandler queryHandler = new QueryHandler();
            return queryHandler.GetSymbolData(symbolData);
        }

        private PresentationRenderer Answer(SearchQuery queryItem, bool managed)
        {
            String searchQuery = queryItem.Query;

            DataModel dataModel = new DataModel();
            bool answered = false;

            String username = queryItem.Username;

            PresentationRenderer presentRender = null;

            if (String.IsNullOrEmpty(searchQuery) == false)
            {
                DateTime start = DateTime.Now;


                QueryHandler queryHandler = new QueryHandler();

                var answerTransferPackage = managed ? queryHandler.SubmitQueryReduce(username, searchQuery) : 
                    queryHandler.SubmitQuery(username, searchQuery);
                if (answerTransferPackage == null)
                {
                    dataModel.LogQuery(username, searchQuery, answered);
                    return null;
                }
                else
                {
                    presentRender = new PresentationRenderer(answerTransferPackage);

                    answered = true;
                    dataModel.LogQuery(username, searchQuery, answered);
                }
                DateTime end = DateTime.Now;
                var timespan = end - start;
                System.Diagnostics.Debug.WriteLine("Deserialsing Timespan :: " 
                    + timespan.TotalSeconds);

                ////Pass 
            }
            return presentRender;
        }

        /// <summary>
        /// Subscribe a user to a continously running query, for immediate result update for when a new answer is available.
        /// </summary>
        /// <param name="subMan">An object that contains the username and query, this is used to subscribe to a continously running query.</param>
        [Route("FollowQuery")]
        [HttpPost]
        public void FollowQuery(SubscripitionManagment subMan)
        {
            DataModel dataModel = new DataModel();
            dataModel.SubscribeToQuery(subMan.UserName, subMan.Query);
        }

        /// <summary>
        /// Unsubscribe a user to a continously running query, for immediate result update for when a new answer is available.
        /// </summary>
        /// <param name="subMan">An object that contains the username and query, this is used to subscribe to a continously running query.</param>
        [Route("UnfollowQuery")]
        [HttpPost]
        public void UnfollowQuery(SubscripitionManagment subMan)
        {
            DataModel dataModel = new DataModel();
            dataModel.UnsubscribeQuery(subMan.UserName, subMan.Query);
        }

        /// <summary>
        /// Save favourite queries.
        /// </summary>
        /// <param name="subMan">An object that contains the username and query, this is used to subscribe to a continously running query.</param>
        [Route("SaveQuery")]
        [HttpPost]
        public void SaveQuery(StoredQueryManagment subMan)
        {
            DataModel dataModel = new DataModel();
            dataModel.SaveUserQuery(subMan.UserName, subMan.Query);
        }

        /// <summary>
        /// Unsave favourite queries.
        /// </summary>
        /// <param name="subMan">An object that contains the username and query, this is used to subscribe to a continously running query.</param>
        [Route("UnsaveQuery")]
        [HttpPost]
        public void UnsaveQuery(StoredQueryManagment subMan)
        {
            DataModel dataModel = new DataModel();
            dataModel.UnsaveUserQueries(subMan.UserName, subMan.Query);
        }


        [Route("GetUserProfile")]
        [HttpPost]
        public PresentationRenderer GetUserProfile(UserNameItem item)
        {
            var presentRender = new PresentationRenderer();

            DataModel dataModel = new DataModel();
            presentRender.UserProfileConfig = dataModel.GetUserProfile(item.UserName);
            return presentRender;
        }  

    }
}
