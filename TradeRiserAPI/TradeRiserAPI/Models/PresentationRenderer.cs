using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace TradeRiserAPI.Models
{
    public struct ResultSummary
    {
        public String SymbolID;
        public String StartDateTime;
        public String EndDateTime;
        public String Source;
        public String TimeFrame;
        public String MoreStandardData;
        public String MoreKeyFields;
        public String QueryID;
        public String Query;

        public List<String> ImageCollection;
        public List<List<String>> KeyResultField;
    }

    public struct QueryCard
    {
        public String QueryID;
        public String Query;
    }

    public class UserProfileConfig
    {
        public List<QueryCard> Following = new List<QueryCard>();
        public List<QueryCard> HistoricQueries = new List<QueryCard>();
        public List<QueryCard> SavedQueries = new List<QueryCard>();

    }



    public class PresentationRenderer
    {
        private QueryService.AnswerPackage currentResult;

        private List<ResultSummary> resultSummaries;

        private String _clientRenderFunction;
        private UserProfileConfig _userProfileConfig;

        public JsonResult JSONData;

        public String ClientRenderFunction
        {
            get { return _clientRenderFunction; }
        }

        public List<ResultSummary> ResultSummaries
        {
            get {return resultSummaries;}
            set { resultSummaries = value;}
        }

        public UserProfileConfig UserProfileConfig
        {
            get { return _userProfileConfig; }
            set { _userProfileConfig = value; }
        }

        public PresentationRenderer()
        {
            resultSummaries = new List<ResultSummary>();
        }

        public PresentationRenderer(QueryService.AnswerPackage results)
        {
            resultSummaries = new List<ResultSummary>();

            Initialize(results);
        }

        public QueryService.AnswerPackage CurrentResult 
        {
            get {return currentResult;}
            set {currentResult = value;}
        }

        public List<String> GetTimePeriod(Queue<QueryService.SessionTransactQQ> results)
        {
            List<String> timePeriod = new List<string>();

            //var dataStr = results.Dequeue().Data;
 
            var dataStr = new String('h', 3);
            timePeriod.Add(dataStr);

            return timePeriod;
        }

        private void Initialize(QueryService.AnswerPackage results)
        {
            try
            {
                if (results != null)
                {
                    currentResult = results;

                    String[][] currentRes = currentResult.ProcessedResults.ComputedResults;
                    var headerResult = currentResult.ProcessedResults.Headers;

                    var currentResT = currentResult.ProcessedResults.ComputedResults.ToDictionary(d => d, d => "")
                        .FirstOrDefault();

                    //row
                    for (int i = 0; i < currentRes.Count(); i++)
                    {
                        if (currentRes[i] != null)
                        {         
                            var item = currentRes[i];

                            ResultSummary resultSum = new ResultSummary();
                            resultSum.KeyResultField = new List<List<String>>();
                            string resultSym = "";
                            string currentMainSymbol = "";

                            for (int j = 0; j < headerResult.Count(); j++)
                            {
                                switch (headerResult[j])
                                {
                                    case "ResultantSymbolID":
                                    case "Resultant Symbol":

                                        {
                                            resultSym = item[j];
                                            resultSum.SymbolID = item[j];
                                            currentMainSymbol = TradeUtility.ConvertSymbolIntoFriendlyForm(item[j]);
                                        }
                                        break;

                                    case "StartDateTime":
                                    case "Start Date Time":
                                        {
                                            resultSum.StartDateTime = item[j];
                                        }
                                        break;

                                    case "EndDateTime":
                                    case "End Date Time":

                                        {
                                            resultSum.EndDateTime = item[j];
                                        }
                                        break;

                                    case "ForexDestination":
                                    case "DestinationExchange":
                                        {
                                            resultSum.Source = item[j];
                                        }
                                        break;

                                    case "TimeFrame":
                                    case "Time Frame":
                                        {
                                            resultSum.TimeFrame = item[j];
                                        }
                                        break;
                                }
                            }


                            foreach (var indexItem in currentResult.ProcessedResults.KeyFieldIndex)
                            {
                                foreach (var indexRes in indexItem)
                                {
                                    resultSum.KeyResultField.Add(new List<String>() { headerResult[indexRes], item[indexRes] });
                                }
                            }

                            if (resultSum.Source == "Forex")
                            {                                
                                if (currentMainSymbol != null)
                                {
                                    if (currentMainSymbol.IndexOf('/') > -1)
                                    {
                                        var symbolList = currentMainSymbol.Split('/');
                                        resultSum.ImageCollection = new List<String>();

                                        foreach (var symItem in symbolList)
                                        {
                                            var path = "../../Images/flagcurrencies/" + symItem.ToLower() + ".png";
                                            resultSum.ImageCollection.Add(path);
                                        }
                                    }
                                }
                            }
                            //resultSum.QueryID = currentResult.QueryID;

                            //cmc i is used to track the items in the query results front end

                            resultSum.QueryID = resultSym + "*" + currentResult.QueryID + "cmc" + i;
                            
                            
                            resultSum.Query = currentResult.Query;


                            //resultSum.QueryID = currentResult.QueryID + "cmc" + i;

                            resultSummaries.Add(resultSum);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            
        }

        public void GenerateDummyUserProfile()
        {
            UserProfileConfig userProfile = new UserProfileConfig();

            QueryCard queryCard = new QueryCard();
            queryCard.QueryID = "MMX001";
            queryCard.Query = "Which currency pairs correlates with the EUR/PLN between 03/01/2013 to 26/03/2013?";
            userProfile.Following.Add(queryCard);


            QueryCard queryCardz = new QueryCard();
            queryCardz.QueryID = "MMX002";
            queryCardz.Query = "Which currency pairs have a high volatility on a 1 minute timeframe" 
            + "and an uptrend on the 4 hour timeframe and sharp drop 2 weeks ago on the 30 minute time frame?";
            userProfile.Following.Add(queryCardz);


            QueryCard queryCardq = new QueryCard();
            queryCardq.QueryID = "MMX003";
            queryCardq.Query = "Which currency pairs have a high volatility on a 1 minute timeframe and "
            + "an uptrend on the 4 hour timeframe and sharp drop 2 weeks ago on the 30 minute time frame?";
            userProfile.Following.Add(queryCardq);


            QueryCard queryCardt = new QueryCard();
            queryCardt.QueryID = "MMX004";
            queryCardt.Query = "Has the AUD/USD ever had a false breakout from its support and resistance line "
            + "whilst GBP/JPY spiked in the same period on a 15 minute time frame?";
            userProfile.Following.Add(queryCardt);
            

            QueryCard queryCardm = new QueryCard();
            queryCardm.QueryID = "MMX004";
            queryCardm.Query = "which currency pair has 20% closing price below the" 
            + " 20 day SMA and correlates usdhuf with the from 03/01/2013 to 26/03/2013?";
            userProfile.Following.Add(queryCardm);


            QueryCard queryCardxx = new QueryCard();
            queryCardxx.QueryID = "NJX001";
            queryCardxx.Query = "Seasonality in EURUSD from 1992 - 2012";
            userProfile.HistoricQueries.Add(queryCardxx);

            QueryCard queryCardxa = new QueryCard();
            queryCardxa.QueryID = "NJX002";
            queryCardxa.Query = "Was the SPY affected by the drop in price of gold 2 weeks ago?";
            userProfile.HistoricQueries.Add(queryCardxa);

            QueryCard queryCardxu = new QueryCard();
            queryCardxu.QueryID = "NJX003";
            queryCardxu.Query = "Which strongly correlating currency pair in the past 3 months " 
            + "has bullish reversal patterns and has three line strike pattern on the 1 minute timeframe in realtime?";
            userProfile.HistoricQueries.Add(queryCardxu);

            _userProfileConfig = userProfile;

        }


        private List<String> GetSymbolIconCollection(String sym, String source)
        {
            List<String> collection = new List<string>();

            if (source == "Forex")
            {
                var symbolList = sym.Split('/');

                foreach (var symItem in symbolList)
                {
                    var path = "../../Images/flagcurrencies/" + symItem.ToLower() + ".png";
                    collection.Add(path);
                }
            }
            return collection;
        }

        public void InitializeContinousResultsDummyData()
        {

            ResultSummary resultSum = new ResultSummary();
            resultSum.SymbolID = "EUR/HUF";
            resultSum.StartDateTime = "02/04/2010 04:15";
            resultSum.EndDateTime = "07/04/2010 03:30";
            resultSum.Source = "Forex";
            resultSum.TimeFrame = "15min";
            resultSum.KeyResultField = new List<List<String>>();
            resultSum.QueryID = "1";

            List<String> tempList = new List<String>() {"Correlation Ratio", "0.356"};
            resultSum.KeyResultField.Add(tempList);

            resultSum.ImageCollection = GetSymbolIconCollection(resultSum.SymbolID, resultSum.Source);
          
            resultSummaries.Add(resultSum);


            ResultSummary resultSumA = new ResultSummary();
            resultSumA.SymbolID = "EUR/USD";
            resultSumA.StartDateTime = "27/03/2010 09:45";
            resultSumA.EndDateTime = "07/04/2010 03:15";
            resultSumA.Source = "Forex";
            resultSumA.TimeFrame = "15min";
            resultSumA.KeyResultField = new List<List<String>>();
            resultSum.QueryID = "2";

            //resultSumA.KeyResultField.Add("Correlation Ratio", "0.6");

            resultSumA.KeyResultField.Add(new List<String>() { "Correlation Ratio", "0.6" });
            resultSumA.ImageCollection = GetSymbolIconCollection(resultSumA.SymbolID, resultSum.Source);

            resultSummaries.Add(resultSumA);


            ResultSummary resultSumB = new ResultSummary();
            resultSumB.SymbolID = "USD/NOK";
            resultSumB.StartDateTime = "03/04/2010 09:30";
            resultSumB.EndDateTime = "07/04/2010 03:15";
            resultSumB.Source = "Forex";
            resultSumB.TimeFrame = "15min";
            resultSumB.KeyResultField = new List<List<String>>();
            resultSumB.QueryID = "3";

            //resultSumB.KeyResultField.Add("Correlation Ratio", "-0.8");
            //resultSumB.KeyResultField.Add("Sharp Rise", "29%");
            resultSumB.KeyResultField.Add(new List<String>() { "Correlation Ratio", "-0.8" });
            resultSumB.KeyResultField.Add(new List<String>() { "Sharp Rise", "29%" });

            resultSumB.ImageCollection = GetSymbolIconCollection(resultSumB.SymbolID, resultSum.Source);
            resultSummaries.Add(resultSumB);


            ResultSummary resultSumC = new ResultSummary();
            resultSumC.SymbolID = "NZD/USD";
            resultSumC.StartDateTime = "03/04/2010 09:30";
            resultSumC.EndDateTime = "09/04/2010 03:15";
            resultSumC.Source = "Forex";
            resultSumC.TimeFrame = "15min";
            resultSumC.KeyResultField = new List<List<String>>();
            resultSum.QueryID = "4";

            resultSumC.MoreStandardData = "...";
            resultSumC.MoreKeyFields = "...";

            //resultSumC.KeyResultField.Add("Correlation Ratio", "-0.8");
            //resultSumC.KeyResultField.Add("P/E Ratio", "58");
            //resultSumC.KeyResultField.Add("Pattern", "Heads and Shoulders");


            resultSumC.KeyResultField.Add(new List<String>() { "Correlation Ratio", "-0.8" });
            resultSumC.KeyResultField.Add(new List<String>() { "P/E Ratio", "58" });
            resultSumC.KeyResultField.Add(new List<String>() { "Pattern", "Heads and Shoulders" });

            resultSumC.ImageCollection = GetSymbolIconCollection(resultSumC.SymbolID, resultSum.Source);
            resultSummaries.Add(resultSumC);


            ResultSummary resultSumD = new ResultSummary();
            resultSumD.SymbolID = "EUR/GBP";
            resultSumD.StartDateTime = "03/04/2010 09:30";
            resultSumD.EndDateTime = "09/04/2010 03:15";
            resultSumD.Source = "Forex";
            resultSumD.TimeFrame = "15min";
            resultSumD.KeyResultField = new List<List<String>>();
            resultSum.QueryID = "5";

            resultSumD.MoreStandardData = "...";
            resultSumD.MoreKeyFields = "...";

            resultSumD.KeyResultField.Add(new List<String>() { "Pattern", "Channel Down" });


            //resultSumD.KeyResultField.Add("Pattern", "Channel Down");
            //resultSumD.KeyResultField.Add("Pattern", "Triangle");
            resultSumD.ImageCollection = GetSymbolIconCollection(resultSumD.SymbolID, resultSum.Source);
            resultSummaries.Add(resultSumD);
        }

        public void InitializeResultDummyData()
        {
            ResultSummary resultSumA = new ResultSummary();
            resultSumA.SymbolID = "EUR/USD";
            resultSumA.StartDateTime = "27/03/2010 09:45";
            resultSumA.EndDateTime = "07/04/2010 03:15";
            resultSumA.Source = "Forex";
            resultSumA.TimeFrame = "15min";
            resultSumA.KeyResultField = new List<List<String>>();
            resultSumA.QueryID = "1x";
            resultSumA.KeyResultField.Add(new List<String>() { "Correlation Ratio", "0.921" });
            resultSummaries.Add(resultSumA);

            ResultSummary resultSumCxx = new ResultSummary();
            resultSumCxx.SymbolID = "CHF/JPY";
            resultSumCxx.StartDateTime = "27/03/2010 09:45";
            resultSumCxx.EndDateTime = "07/04/2010 03:15";
            resultSumCxx.Source = "Forex";
            resultSumCxx.TimeFrame = "15min";
            resultSumCxx.KeyResultField = new List<List<String>>();
            resultSumCxx.QueryID = "2x";
            resultSumCxx.KeyResultField.Add(new List<String>() { "Correlation Ratio", "0.651" });
            resultSummaries.Add(resultSumCxx);

            ResultSummary resultSumZxx = new ResultSummary();
            resultSumZxx.SymbolID = "USD/CAD";
            resultSumZxx.StartDateTime = "27/03/2010 09:45";
            resultSumZxx.EndDateTime = "07/04/2010 03:15";
            resultSumZxx.Source = "Forex";
            resultSumZxx.TimeFrame = "15min";
            resultSumZxx.KeyResultField = new List<List<String>>();
            resultSumZxx.QueryID = "3x";
            resultSumZxx.KeyResultField.Add(new List<String>() { "Correlation Ratio", "0.345" });
            resultSummaries.Add(resultSumZxx);

            ResultSummary resultSumNxx = new ResultSummary();
            resultSumNxx.SymbolID = "EUR/CHF";
            resultSumNxx.StartDateTime = "27/03/2010 09:45";
            resultSumNxx.EndDateTime = "07/04/2010 03:15";
            resultSumNxx.Source = "Forex";
            resultSumNxx.TimeFrame = "15min";
            resultSumNxx.KeyResultField = new List<List<String>>();
            resultSumNxx.QueryID = "4x";
            resultSumNxx.KeyResultField.Add(new List<String>() { "Correlation Ratio", "0.425" });
            resultSummaries.Add(resultSumNxx);

            ResultSummary resultSum = new ResultSummary();
            resultSum.SymbolID = "EUR/JPY";
            resultSum.StartDateTime = "02/04/2010 04:15";
            resultSum.EndDateTime = "07/04/2010 03:30";
            resultSum.Source = "Forex";
            resultSum.TimeFrame = "15min";
            resultSum.KeyResultField = new List<List<String>>();
            resultSum.QueryID = "5x";
            List<String> tempList = new List<String>() { "Correlation Ratio", "0.356" };
            resultSum.KeyResultField.Add(tempList);
            resultSummaries.Add(resultSum);

            ResultSummary resultSumB = new ResultSummary();
            resultSumB.SymbolID = "USD/NOK";
            resultSumB.StartDateTime = "03/04/2010 09:30";
            resultSumB.EndDateTime = "07/04/2010 03:15";
            resultSumB.Source = "Forex";
            resultSumB.TimeFrame = "15min";
            resultSumB.KeyResultField = new List<List<String>>();
            resultSumB.QueryID = "6x";
            resultSumB.KeyResultField.Add(new List<String>() { "Correlation Ratio", "-0.8" });
            resultSummaries.Add(resultSumB);

            ResultSummary resultSumC = new ResultSummary();
            resultSumC.SymbolID = "NZD/USD";
            resultSumC.StartDateTime = "03/04/2010 09:30";
            resultSumC.EndDateTime = "09/04/2010 03:15";
            resultSumC.Source = "Forex";
            resultSumC.TimeFrame = "15min";
            resultSumC.KeyResultField = new List<List<String>>();
            resultSum.QueryID = "7x";
            resultSumC.KeyResultField.Add(new List<String>() { "Correlation Ratio", "-0.9" });
            resultSummaries.Add(resultSumC);
        }

        public void RunResultDummyData()
        {
            ResultSummary resultSumA = new ResultSummary();
            resultSumA.SymbolID = "EUR/USD : " + DateTime.Now.ToString();
            resultSumA.StartDateTime = "27/03/2010 09:45";
            resultSumA.EndDateTime = "07/04/2010 03:15";
            resultSumA.Source = "Forex";
            resultSumA.TimeFrame = "15min";
            resultSumA.KeyResultField = new List<List<String>>();
            resultSumA.QueryID = DateTime.Now.ToString();
            resultSumA.KeyResultField.Add(new List<String>() { "Correlation Ratio", "0.921" });
            resultSummaries.Add(resultSumA);

        }  
    }



    /// <summary>
    /// JSON Serialization and Deserialization Assistant Class
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// JSON Serialization
        /// </summary>
        public static string JsonSerializer<T>(T t)
        {

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            //Replace Json Date String                                         
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString)
        {
            //Convert "yyyy-MM-dd HH:mm:ss" String as "\/Date(1319266795390+0800)\/"
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(
                ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>
        /// Convert Serialization Time /Date(1319266795390+0800) as String
        /// </summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// Convert Date String as Json Time
        /// </summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime LastLoginTime { get; set; }
    }


}