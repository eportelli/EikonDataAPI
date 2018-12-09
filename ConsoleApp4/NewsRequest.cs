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
    class NewsRequest
    {
        public void RetrieveNews(string NewsCode)
        {
            IEikon eikon = Eikon.CreateDataAPI();
            eikon.SetAppKey(Program.AppKey);

            try
            {
                var response = eikon.GetNewsHeadlinesRaw(NewsCode, 100);
                var storyId = JsonConvert.DeserializeObject<NewsHeadlinesResponse>(response);
                //var headline = JsonConvert.DeserializeObject<NewsHeadline>(storyId.headlines.ToString());
                //response.Print();


                //Get latest Version creates System.DateTime '

                NewsStoryResponse[] storydata = new NewsStoryResponse[10];
                for (int a = 9; a >= 0; --a)
                {
                    //Check if storyId.headlines[a].versionCreated is newer then stored System.DateTime
                    DateTime CurrentHLUpdate = storyId.headlines[a].versionCreated;
                    string newstext = storyId.headlines[a].text;

                    if (globals.LastHeadlineUpdate < CurrentHLUpdate && !newstext.Substring(0, 10).Equals("*TOP NEWS*"))
                    {
                        var storyresponse = eikon.GetNewsStoryRaw(storyId.headlines[a].storyId);
                        storydata[a] = JsonConvert.DeserializeObject<NewsStoryResponse>(storyresponse);
                        Console.WriteLine(storydata[a].story.headlineHtml);
                        var dt = storydata[a].story.storyInfoHtml;
                        globals.LastHeadlineUpdate = CurrentHLUpdate;
                        using (SqlConnection myConnection = Program.getConnection())
                        {
                            if (myConnection.State == ConnectionState.Closed)
                            {
                                myConnection.Open();
                            }
                            string query = "INSERT INTO dbo.ReutersNews (ReutersID, StoryDate, Title, Summary) " +
                               "Values (@ReutersID, @StoryDate, @Title, @Summary)";
                            SqlCommand cmd = new SqlCommand(query, myConnection);
                            cmd.Parameters.AddWithValue("@ReutersId", storyId.headlines[a].storyId);
                            cmd.Parameters.AddWithValue("@StoryDate", storyId.headlines[a].versionCreated);
                            cmd.Parameters.AddWithValue("@Title", storyId.headlines[a].text);
                            cmd.Parameters.AddWithValue("@Summary", storydata[a].story.storyHtml);
                            cmd.ExecuteNonQuery();
                        }

                    }
                }

            }
            catch (EikonException ex)
            {
                Console.WriteLine("News retrieval error");
            }

        }
    }
}
