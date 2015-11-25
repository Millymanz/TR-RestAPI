using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;

using Microsoft.Win32;
using System.Timers;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using PusherServer;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Net;
using System.Xml.XPath;


using System.Web;

namespace TradeRiserAPI.Models
{
    public class RestClient
    {
        public string GetAccessToken(string username, string password, HttpRequest requestOb)
        {
            var url = requestOb.Url.GetLeftPart(UriPartial.Authority) +"/token";

            WebRequest request = WebRequest.Create(url);
            // Set the Method property of the request to POST.
            request.Method = "POST";

            string postData = "username=" + username + "&grant_type=password" + "&password=" + password;


            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();



            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseFromServer);
            return (string)json["access_token"];
        }
    }


    public class BroadcastorCallback : QueryPushService.IQueryPushServiceCallback
    {
        private System.Threading.SynchronizationContext syncContext = AsyncOperationManager.SynchronizationContext;

        private EventHandler _broadcastorCallBackHandler;
        public void SetHandler(EventHandler handler)
        {
            this._broadcastorCallBackHandler = handler;
        }

        public void BroadcastToClient(QueryPushService.EventDataType eventData)
        {
            syncContext.Post(new System.Threading.SendOrPostCallback(OnBroadcast), eventData);
        }

        private void OnBroadcast(object eventData)
        {
            this._broadcastorCallBackHandler.Invoke(eventData, null);
        }
    }


    public static class Poller
    {
        static Dictionary<String, Queue<QueryService.SessionTransactQQ>> queryDataMasterNode = new Dictionary<string, Queue<QueryService.SessionTransactQQ>>();

        public static Queue<QueryService.AnswerPackage> answerQueue = new Queue<QueryService.AnswerPackage>();

        //private static QueryService.QueryServiceClient _client;
        private static QueryPushService.QueryPushServiceClient _client;


        public static void Initialise()
        {

            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }

            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GETANSWERPORT"].ToString());

            while (true)
            {
                UdpClient udpClient = new UdpClient(port);//port
                try
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(localIP), port);

                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, port);

                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    if (returnData == "answerReady")
                    {
                        List<QueryService.AnswerPackage> answers = GetLatestAnswers();

                        foreach (var item in answers)
                        {
                            answerQueue.Enqueue(item);
                        }
                    }
                    //udpClient.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    udpClient.Close();
                }
            }
        }

        public static void InitialiseClient()
        {
            RegisterClient();
        }     

        public static List<QueryService.AnswerPackage> GetLatestAnswers()
        {
            //QueryService.AnswerPackage answerPackage = null;

            //BroadcastorCallback cb = new BroadcastorCallback();
            //cb.SetHandler(Poller.HandleBroadcast);

            //System.ServiceModel.InstanceContext context =
            //    new System.ServiceModel.InstanceContext(cb);


            //using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient(context))
            //{
            //    return queryServiceProxy.GetLatestAnswers().ToList();
            //}
            return null;
        }

        public static Queue<QueryService.SessionTransactQQ> GetQueryData(String userQuerySessionID)
        {
            //Search dictionary

            Queue<QueryService.SessionTransactQQ> stQueue = null;

            queryDataMasterNode.TryGetValue(userQuerySessionID, out stQueue);

            return stQueue;
        }


        private static void RegisterClient()
        {
            try
            {
                if ((_client != null))
                {
                    _client.Abort();
                    _client = null;
                }

                BroadcastorCallback cb = new BroadcastorCallback();
                cb.SetHandler(HandleBroadcast);

                System.ServiceModel.InstanceContext context =
                    new System.ServiceModel.InstanceContext(cb);
                _client =
                    new QueryPushService.QueryPushServiceClient(context);

                //new QueryService.QueryServiceClient(context);

                

                Logger.log.Info("TradeRiserAnalytics - Registering For Push");
                
                String name = Dns.GetHostName().ToUpper();
                _client.RegisterClient(name +"_TradeRiserAnalytics");
            }
            catch (Exception ex)
            {
                Logger.log.Error("Push Registeration Failed : " + ex);
            }
        }

        //-----------------------------------------------------------------------------------------------//

        #region "callback services"

        private delegate void HandleBroadcastCallback(object sender, EventArgs e);

        public static void HandleBroadcast(object sender, EventArgs e)
        {
            try
            {
                //see if object can be set or created as AnswerPackage
                var eventData = (QueryPushService.EventDataType)sender;

                AutoMapper.Mapper.CreateMap<QueryPushService.ChartDataPairing, QueryService.ChartDataPairing>();
                AutoMapper.Mapper.CreateMap<QueryPushService.ComputedResultTable, QueryService.ComputedResultTable>();
                AutoMapper.Mapper.CreateMap<QueryPushService.RawDataResultTable, QueryService.RawDataResultTable>();
                AutoMapper.Mapper.CreateMap<QueryPushService.PresentationPackage, QueryService.PresentationPackage>();
                AutoMapper.Mapper.CreateMap<QueryPushService.HighlightRegion, QueryService.HighlightRegion>();

                AutoMapper.Mapper.CreateMap<QueryPushService.AnswerPackage, QueryService.AnswerPackage>();

                QueryService.AnswerPackage answerPackageConverted = AutoMapper.Mapper.Map<QueryService.AnswerPackage>(eventData.EventMessage);

                //QueryService.EventDataType queryServiceItem = new QueryService.EventDataType();
                //queryServiceItem.ClientName = eventData.ClientName;
                //queryServiceItem.EventMessage = answerPackageConverted;

                var presentRender = new PresentationRenderer(answerPackageConverted);

                //presentRender.RunResultDummyData();//test

               
                var pusher = new Pusher("121338", "0c52bffe086a83952d16", "112e5c6c8c427bc4788a");

                var result = pusher.Trigger(eventData.EventMessage.UserQuerySessionID, "my_event", presentRender.ResultSummaries.FirstOrDefault());


                //Pass 


               // System.Diagnostics.Debug.WriteLine(tempText); // 2
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        #endregion

    }
}