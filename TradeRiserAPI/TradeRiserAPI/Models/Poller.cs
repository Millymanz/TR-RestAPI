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


namespace TradeRiserAPI.Models
{

    public class BroadcastorCallback : QueryService.IQueryServiceCallback
    {
        private System.Threading.SynchronizationContext syncContext = AsyncOperationManager.SynchronizationContext;

        private EventHandler _broadcastorCallBackHandler;
        public void SetHandler(EventHandler handler)
        {
            this._broadcastorCallBackHandler = handler;
        }

        public void BroadcastToClient(QueryService.EventDataType eventData)
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

        private static QueryService.QueryServiceClient _client;


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
            QueryService.AnswerPackage answerPackage = null;

            BroadcastorCallback cb = new BroadcastorCallback();
            cb.SetHandler(Poller.HandleBroadcast);

            System.ServiceModel.InstanceContext context =
                new System.ServiceModel.InstanceContext(cb);


            using (QueryService.QueryServiceClient queryServiceProxy = new QueryService.QueryServiceClient(context))
            {
                return queryServiceProxy.GetLatestAnswers().ToList();
            }
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
                new QueryService.QueryServiceClient(context);

            _client.RegisterClient("TradeRiserAnalytics");
        }

        //-----------------------------------------------------------------------------------------------//

        #region "callback services"

        private delegate void HandleBroadcastCallback(object sender, EventArgs e);

        public static void HandleBroadcast(object sender, EventArgs e)
        {
            try
            {
                //see if object can be set or created as AnswerPackage
                var eventData = (QueryService.EventDataType)sender;
                var presentRender = new PresentationRenderer(eventData.EventMessage);

                presentRender.RunResultDummyData();//test

               
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