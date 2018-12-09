using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EikonDataAPI;
using Deedle;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ConsoleApp4
{
    class Program
    {
        public static string AppKey = "1dc274d42f7044a799a86fb2b0533eeca43f2e07";
        public static string ConnString = "";

        static void Main(string[] args)
        {

            
            DataRequests dr = new DataRequests();
            dr.RetrieveData(globals.rics, globals.Fields);


            NewsRequest ns = new NewsRequest();
            ns.RetrieveNews("TOP/G AND LEN");
            Timer t = new System.Threading.Timer(TimerCommand, null, 0, 60000);

            Console.Read();





        }




        static void DataFeed(string[] RicCodes)
        {



        }
        public static SqlConnection getConnection()
        {
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = "Server = tcp:mtweb3db.database.windows.net,1433; Initial Catalog = mtweb3db; Persist Security Info = False; User ID = MICAdmin; Password = Malta01!; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;" ;
            return myConnection;


        }

        private static void TimerCommand(Object o)
        {
            NewsRequest nr = new NewsRequest();
            Console.WriteLine("Checking News : {0}", DateTime.Now.ToString());
            nr.RetrieveNews("TOP/G AND LEN");

            DataRequests dr = new DataRequests();
            dr.RetrieveData(globals.rics, globals.Fields);
            Console.WriteLine("Checking and updating data : {0}", DateTime.Now.ToString());
        }

    }

public class NewsHeadlinesResponse
    {
        public List<NewsHeadline> headlines;
        public string newer;
        public string older;

    }

    public class NewsHeadline
    {
        public string author;
        public string brokerName;
        public string displayDirection;
        public string documentID;
        public string documentType;
        public DateTime firstCreated;
        public DateTime versionCreated;
        public string headlineType;
        public bool isAlert;
        public string language;
        public string numberOfPages;
        public string reportCode;
        public string sourceCode;
        public string sourceName;
        public string storyId;
        public string text;

    }
    internal class NewsHeadlinesRequest
    {
        public string query;
        public string number;
        public string dateFrom;
        public string dateTo;
        // public string timezone;
        //[JsonConverter(typeof(StringEnumConverter))]
        [JsonConverter(typeof(ListEnumToCsvConverter<NewsRepository>))]
        public List<NewsRepository> repository;
        public string payload;
    }
    public enum NewsRepository { NewsWire, NewsRoom, WebNews }

    internal class NewsStoryRequest
    {
        public string storyId;
    }
    public class NewsStoryResponse
    {
        public Story story;
    }
    public class Story
    {
        public string headlineHtml;
        public string storyHtml;
        public string storyInfoHtml;
    }

    public class globals
    {
        public static DateTime LastHeadlineUpdate;
        public static List<string> rics = new List<string>
            {
                "TRI.N", "BAC", "IBM.N", "T.N", "VOD.L", "BARC.L", "6758.T"
            };

        public static List<string> Fields = new List<string>
            {
                "DSPLY_NAME","TRDPRC_1","NETCHNG_1", "PCTCHNG", "TRADE_DATE", "TRDTIM_1"
            };
    }


}
