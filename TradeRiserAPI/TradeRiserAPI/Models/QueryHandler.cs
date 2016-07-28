using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TradeRiserAPI.Models
{
    public class QueryHandler
    {
        //public QueryService.SessionTransact Submit(String userQuerySession, String query)
        //{
        //    BroadcastorCallback cb = new BroadcastorCallback();
        //    cb.SetHandler(Poller.HandleBroadcast);

        //    System.ServiceModel.InstanceContext context =
        //        new System.ServiceModel.InstanceContext(cb);


        //    //QueryServiceServiceClient fg = new QueryServiceServiceClient();
        //    using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient(context))
        //    {
        //        QueryService.SessionTransact res = queryServiceProxy.Submit(userQuerySession, query);
        //        return res;
        //    }
        //}

        public QueryService.AnswerPackage GetAllCompletedPatternDefaults()
        {
            try
            {
                //BroadcastorCallback cb = new BroadcastorCallback();
                //cb.SetHandler(Poller.HandleBroadcast);

                //System.ServiceModel.InstanceContext context =
                //    new System.ServiceModel.InstanceContext(cb);

                using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient())
                {
                    return queryServiceProxy.GetAllCompletedPatternDefaults("Forex");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Fault :: " + ex.ToString());
            }
            return null;
        }

        public QueryService.AnswerPackage GetAllEmergingPatternDefaults()
        {
            try
            {
                //BroadcastorCallback cb = new BroadcastorCallback();
                //cb.SetHandler(Poller.HandleBroadcast);

                //System.ServiceModel.InstanceContext context =
                //    new System.ServiceModel.InstanceContext(cb);

                using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient())
                {
                    return queryServiceProxy.GetAllEmergingPatternDefaults("Forex");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Fault :: " + ex.ToString());
            }
            return null;
        }     

        public QueryService.AnswerPackage SubmitQuery(String userQuerySession, String query)
        {
            return Query(userQuerySession, query);
        }

        //public List<List<double>> GetSymbolData(SymbolData symbolData)
        //public QueryService.ChartDataPairing GetSymbolData(SymbolData symbolData)
        //{
        //    try
        //    {
        //        BroadcastorCallback cb = new BroadcastorCallback();
        //        cb.SetHandler(Poller.HandleBroadcast);

        //        System.ServiceModel.InstanceContext context =
        //            new System.ServiceModel.InstanceContext(cb);

        //        using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient(context))
        //        {
        //            var data = queryServiceProxy.GetSymbolData(symbolData.SymbolID, symbolData.TimeFrame);
                    
        //            QueryService.ChartDataPairing chartPairing = new QueryService.ChartDataPairing();
        //            chartPairing.Key = "RAW";
        //            chartPairing.Value = data;

        //            return chartPairing;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Connection Fault :: " + ex.ToString());
        //    }
        //    return null;
        //}

        public bool GetBackEndBasicServiceStatus()
        {
            try
            {
                using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient())
                {
                    return queryServiceProxy.GetBasicServiceStatus();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Fault :: " + ex.ToString());
            }
            return false;
        }

        public string GetBackEndFullServiceStatus()
        {
            try
            {
                using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient())
                {
                    return queryServiceProxy.GetFullServiceStatus();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Fault :: " + ex.ToString());
            }
            return "";
        }

        
        public double[][] GetSymbolData(SymbolData symbolData)
        {
            try
            {
                //BroadcastorCallback cb = new BroadcastorCallback();
                //cb.SetHandler(Poller.HandleBroadcast);

                //System.ServiceModel.InstanceContext context =
                //    new System.ServiceModel.InstanceContext(cb);

                using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient())
                {
                    var data = queryServiceProxy.GetSymbolData(symbolData.SymbolID, symbolData.TimeFrame);
                    return data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Fault :: " + ex.ToString());
            }
            return null;
        }

        public QueryService.AnswerPackage SubmitQueryReduce(String userQuerySession, String query)
        {
            var dataSetMgr = new DatasetManager();

            var ans = Query(userQuerySession, query);
            
            if (ans != null) dataSetMgr.Reduce(ans);

            return ans;
        }

        private QueryService.AnswerPackage Query(String userQuerySession, String query)
        {
            try
            {
                var dataSetMgr = new DatasetManager();

                QueryService.AnswerPackage answerTemp = null;

               // BroadcastorCallback cb = new BroadcastorCallback();
                //// cb.SetHandler(Poller.HandleBroadcast);

                //System.ServiceModel.InstanceContext context =
                //    new System.ServiceModel.InstanceContext(cb);

                using (QueryService.QueryServiceClient queryServiceProxy
                    = new QueryService.QueryServiceClient())
                {
                    answerTemp = queryServiceProxy.SubmitQuery(userQuerySession, query);

                    return answerTemp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Fault :: " + ex.ToString());
            }
            return null;
        }  


        //private QueryService.AnswerPackage Query(String userQuerySession, String query)
        //{
        //    try
        //    {
        //        var dataSetMgr = new DatasetManager();

        //        QueryService.AnswerPackage answerTemp = null;

        //        BroadcastorCallback cb = new BroadcastorCallback();
        //       // cb.SetHandler(Poller.HandleBroadcast);

        //        System.ServiceModel.InstanceContext context =
        //            new System.ServiceModel.InstanceContext(cb);

        //        using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient(context))
        //        {
        //            answerTemp = queryServiceProxy.SubmitQuery(userQuerySession, query);

        //            return answerTemp;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Connection Fault :: " + ex.ToString());
        //    }
        //    return null;
        //}  

        public double[][] GetChartData(String symbolID)
        {
            //BroadcastorCallback cb = new BroadcastorCallback();
            //cb.SetHandler(Poller.HandleBroadcast);

            //System.ServiceModel.InstanceContext context =
            //    new System.ServiceModel.InstanceContext(cb);
            
            using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient())
            {
                return queryServiceProxy.GetChartData(symbolID);
            }
        }

        public QueryService.AnswerPackage GetDataResult(String queryId, String selectingSymbol)
        {
            //BroadcastorCallback cb = new BroadcastorCallback();
            //cb.SetHandler(Poller.HandleBroadcast);

            //System.ServiceModel.InstanceContext context =
            //    new System.ServiceModel.InstanceContext(cb);

            var pureQueryId = queryId.Split(new string[] { "cmc" }, StringSplitOptions.None).FirstOrDefault();

            using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient())
            {
                return queryServiceProxy.GetDataResult(pureQueryId, selectingSymbol);
            }
        }

    }
}