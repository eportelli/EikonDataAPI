﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using EikonDataAPI;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApp4
{
    class DataRequests
    {
        public void RetrieveData(List<string> RicCodes, List<string> FieldList)
        {

            IEikon eikon = Eikon.CreateDataAPI();
            eikon.SetAppKey(Program.AppKey);

            try
            {
                var data = eikon.GetDataRaw(RicCodes, FieldList);
                var payload = JsonConvert.DeserializeObject<DataResponse>(data);

                using (SqlConnection myConnection = Program.getConnection())
                {
                    if (myConnection.State == ConnectionState.Closed)
                    {
                        myConnection.Open();
                    }
                    string query = "INSERT INTO dbo.ReutersData (Ric, DispName, Last, NetChng, PctChng, Date, UpTime) " +
                       "Values (@Ric, @DispName, @Last, @NetChng, @PctChng, @Date, @UpTime)";
                    for (int a = 1; a <= payload.totalRowsCount; a++)
                    {
                        try
                        {
                            var RetData = payload.data[a].ToList();

                           //check the latest date and time value stored in the database
                            SqlCommand cmd = new SqlCommand(query, myConnection);
                            cmd.Parameters.AddWithValue("@Ric", RetData[0].Value);
                            cmd.Parameters.AddWithValue("@DispName", RetData[1].Value);
                            cmd.Parameters.AddWithValue("@Last", RetData[2].Value);
                            cmd.Parameters.AddWithValue("@NetChng", RetData[3].Value);
                            cmd.Parameters.AddWithValue("@PctChng", RetData[4].Value);
                            cmd.Parameters.AddWithValue("@Date", RetData[5].Value);
                            cmd.Parameters.AddWithValue("@UpTime", RetData[6].Value);
                            cmd.ExecuteNonQuery();

                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine("SQL Exception : {0}", ex.ErrorCode.ToString());
                            break;
                        }
                    }
                }


            }
            catch (EikonException ex)
            {
            }
        }
    }
    public class DataResponse
    {
        public int columnHeadersCount;
        public int rowHeadersCount;
        public int totalColumnsCount;
        public int totalRowsCount;
        public string headerOrientation;
        public List<List<Column>> headers;
        public List<List<JValue>> data;

    }
}
