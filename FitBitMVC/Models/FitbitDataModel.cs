using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Owin;
using Owin.Security.Providers.Fitbit.Provider;

namespace FitBitMVC.Models
{
    public class FitbitDataModel
    {
        private static string ConnectionString = @"User ID = Fitbit; Password=FitBit1;Initial Catalog = Fitbit; Server=Ronbox\SQLEXPRESS";

        static FitbitDataModel()
        {
            
        }

        public void UpdatePariticpant(FitbitCredentials userCredentials)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "";
                int result = 0;
                
                try
                {
                    sql = $"select count(*) from [Fitbit].[dbo].[Participants] where [FitbitID] = '{userCredentials.UserID}'";
                    SqlCommand command = new SqlCommand(sql, conn);
                    result = (int)command.ExecuteScalar();

                    if (result > 0)
                    {
                        sql = $"UPDATE [Fitbit].[dbo].[Participants] " +
                              $"SET [AccessToken] = '{userCredentials.AccessToken}', [Name] = '{userCredentials.Name}', [RefreshToken] = '{userCredentials.RefreshToken}'" +
                              $"WHERE [FitbitID] = '{userCredentials.UserID}'";
                    }
                    else
                    {    
                        sql = $"insert into [Fitbit].[dbo].[Participants] ([FitbitID], [AccessToken], [RefreshToken], [Name]) values ('{userCredentials.UserID}', '{userCredentials.AccessToken}', '{userCredentials.RefreshToken}', '{userCredentials.Name}')";
                    }

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    result = cmd.ExecuteNonQuery();

                }
                catch (SqlException ex)
                {
                    Trace.WriteLine($"Sql exception: {ex.Message}");
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Other exception: {e.Message}");
                }

                if (result != 1)
                {
                    Trace.WriteLine("Something went wrong...");
                }
            }
        }

        public FitbitCredentials GetCredentials(string userID)
        {
            var creds = new FitbitCredentials();
            
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "";
                int result = 0;

                try
                {
                    sql = $"select [NAME], [ACCESSTOKEN], [REFRESHTOKEN], [TIMESTAMP] from [Fitbit].[dbo].[Participants] where [FitbitID] = '{userID}'";
                    SqlCommand command = new SqlCommand(sql, conn);
                    var rdr = command.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read()) //need to handle somehow cases of multiple userIDs in the DB?
                        {
                            creds.UserID = userID;
                            creds.Name = rdr.GetString(0);
                            creds.AccessToken = rdr.GetString(1);
                            creds.RefreshToken = rdr.GetString(2);
                            creds.TimeStamp = rdr.GetDateTime(3);
                        }   
                    }


                }
                catch (SqlException ex)
                {
                    Trace.WriteLine($"Sql exception: {ex.Message}");
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Other exception: {e.Message}");
                }

                if (result != 1)
                {
                    Trace.WriteLine("Something went wrong...");
                }
            }

            return creds;
        }
    }

    [DataContract]
    public class FitbitCredentials
    {
        [DataMember(Name = "user_id")]
        public string UserID { get; set; }
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}