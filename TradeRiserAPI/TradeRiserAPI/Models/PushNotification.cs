using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;
using TradeRiserAPI.Models;

namespace TradeRiserAPI.Models
{
    //public static class PushNotification
    //{
    //    public static void Start_PokeIn(String userQuerySessionID)
    //    {

    //        CometWorker.OnClientConnected += new DefineClassObjects(CometWorker_OnClientConnected);

    //        CometWorker.OnClientCreated += new ClientCreatedDelegate(CometWorker_OnClientCreated);

    //    }
        
    //    public static void Push(PresentationRenderer resultData, String pushClient)
    //    {
    //        //string messageT = JSON.Method("UpdatePrices", "Stock1", resultData.ResultSummaries[0].SymbolID);

    //        string messageT = JSON.Method("UpdateContinousQueryResultCard", resultData.ResultSummaries[0]);
    //        //CometWorker.Groups.Send("Stock1", messageT);

    //        CometWorker.Groups.Send(pushClient, messageT);

    //    }

    //    public static void ReverseAjax(Object userQuerySessionID)
    //    {
    //        Random rndPrices = new Random();
    //        while (true)
    //        {
    //            //Need filter to determine which data to send to which client
    //            //*DOA - Get user creditenial session

    //            String uQS = userQuerySessionID.ToString();

    //            if (String.IsNullOrEmpty(uQS) == false)
    //            {
    //                Queue<QueryService.SessionTransactQQ> results = Poller.GetQueryData(uQS);

    //                if (results != null)
    //                {
    //                    //Trickle data
    //                    PresentationRenderer presentationRenderer = new PresentationRenderer();
                        
    //                    var timePeriodList = presentationRenderer.GetTimePeriod(results);

    //                    for (int i = 0; i < timePeriodList.Count; i++)
    //                    {
    //                        //string message = JSON.Method("UpdatePrices", "Stock1", rndPrices.Next(0, 99));
    //                        string message = JSON.Method("UpdatePrices", "Stock1", timePeriodList[i]);
    //                        CometWorker.Groups.Send("Stock1", message);
    //                    }
    //                }
    //            }

    //            string messageT = JSON.Method("UpdatePrices", "Stock1", "Test");
    //            CometWorker.Groups.Send("Stock1", messageT);

    //            Thread.Sleep(1000);
    //            //Generate a new random stock price every 1 second and send it to the channel
    //            //Deploy this test application onto IIS to get better results. for example, try for 0.2 second
    //        }
    //    }

    //    static void CometWorker_OnClientConnected(ConnectionDetails details, ref Dictionary<string, object> classList)
    //    {
    //        //There is a new connection attempt from the browser side and
    //        //PokeIn wants you to define a class for this connection request
    //        classList.Add("StockDemo", new MyStockDemo(details.ClientId));
    //        //Please notice that the connection has not completed in this step yet.
    //        //If you need the exact moment of "client connection completed" then you should be listening OnClientCreated event
    //    }

    //    static void CometWorker_OnClientCreated(string clientId)
    //    {
    //        //Client connection is done
    //        string message = JSON.Method("UpdateString", "Now, you are connected!");
    //        CometWorker.SendToClient(clientId, message);
    //    }

    //}

    //public class MyStockDemo : IDisposable
    //{
    //    private string PokeInClassID = "R821g321L0(8H0o5L0W4R8k1R817T9#2u072w921H5O6J9S0S0V3o5D8!2s1s2s1j0b4f1t7j0f4U6M2U6t7s7j3!1b2s0s1t8j6s2U0U0e3t7f8!2s1s2s1j0b4f1t7j0f4U6M2U6t7s7j3!1b2s0s1t8j6s2U0U0e3t7f8";
    //    string _clientId;
    //    public MyStockDemo(string clientId)
    //    {
    //        _clientId = clientId;
    //    }

    //    public void Dispose()
    //    {
    //        //PokeIn will call this method after client is disconnected
    //    }

    //    //it is not important how many client is listening for this channel. Let PokeIn manage the messages
    //    //please notice that the FREE edition of PokeIn supports up to 10 concurrent connection
    //    //Get Commercial edition to break the limits
    //    //public void JoinChannel()
    //    //{

    //    //    CometWorker.Groups.PinClientID(_clientId, "Stock1");
    //    //    CometWorker.SendToClient(_clientId, "Pinned();");
    //    //}

    //    public void JoinChannel(string name)
    //    {
    //        //CometWorker.Groups.PinClientID(_clientId, "Stock1");

    //        CometWorker.Groups.PinClientID(_clientId, name);

    //        CometWorker.SendToClient(_clientId, "Pinned();");
    //    }

    //    public void LeaveChannel()
    //    {
    //        CometWorker.Groups.UnpinClient(_clientId, "Stock1");
    //        CometWorker.SendToClient(_clientId, "Unpinned();");
    //    }
    //}

}