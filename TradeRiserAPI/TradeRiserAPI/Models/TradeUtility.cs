using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.Xml;

namespace TradeRiserAPI.Models
{
    public static class TradeUtility
    {
        private static Dictionary<String, String> _ForexSymbolLookUp = new Dictionary<String, String>();
        public static Dictionary<string, string> CountryLookupEconomic = new Dictionary<string, string>();

        static TradeUtility()
        {
        }

        public static void Setup()
        {
            var path = System.Configuration.ConfigurationManager.AppSettings["ForexList"];
            using (StreamReader sr = new StreamReader(path))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line) == false)
                    {
                        var symbolArray = line.Split('/');
                        var key = symbolArray[0] + symbolArray[1];
                        _ForexSymbolLookUp.Add(key, line);
                    }
                }
            }

            path = System.Configuration.ConfigurationManager.AppSettings["CountryList"];
            using (StreamReader sr = new StreamReader(path))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line) == false)
                    {
                        var symbolArray = line.Split('/');

                        CountryLookupEconomic.Add(symbolArray[0], symbolArray[1]);
                    }
                }
            }
        }

        public static String ConvertSymbolIntoFriendlyForm(String sym)
        {
            string friendlyVersion = "";
            
            _ForexSymbolLookUp.TryGetValue(sym, out friendlyVersion);

           return friendlyVersion;
        }
    }
}