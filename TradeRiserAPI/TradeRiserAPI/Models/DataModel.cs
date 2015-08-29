using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;


namespace TradeRiserAPI.Models
{
    public class DataModel
    {
        public UserProfileConfig GetUserProfile(String username)
        {
            UserProfileConfig userProfileConfig = new UserProfileConfig();

            //change this code so that in future it only makes one trip to the database

            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];
            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_ViewUserQuerySubscription", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@username", username);

                    SqlDataReader rdr = sqlCommand.ExecuteReader();
                    while (rdr.Read())
                    {
                        var queryCard = new QueryCard();

                        queryCard.QueryID = rdr["QueryLogId"].ToString();
                        queryCard.Query = rdr["Query"].ToString();

                        userProfileConfig.Following.Add(queryCard);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }



            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_ViewUserQueries", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@username", username);

                    SqlDataReader rdr = sqlCommand.ExecuteReader();

                    while (rdr.Read())
                    {
                        var queryCard = new QueryCard();

                        queryCard.QueryID = rdr["QueryLogId"].ToString();
                        queryCard.Query = rdr["Query"].ToString();

                        userProfileConfig.HistoricQueries.Add(queryCard);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }


            return userProfileConfig;
        }

        public bool CreateUserAndAccount(RegisterModel model)
        {
            String saltString = "Generatedinordertochangetheworldweknow934";
            var salt = System.Text.Encoding.UTF8.GetBytes(saltString);
            var password = System.Text.Encoding.UTF8.GetBytes(model.Password);

            var hmacMD5 = new HMACMD5(salt);
            var saltedHash = hmacMD5.ComputeHash(password);

            string hexString = DataModel.ToHex(saltedHash, false);

            //create the MD5CryptoServiceProvider object we will use to encrypt the password
            var md5Hasher = new MD5CryptoServiceProvider();
            //create an array of bytes we will use to store the encrypted password
            Byte[] hashedBytes;

            //Create a UTF8Encoding object we will use to convert our password string to a byte array
            var encoder = new UTF8Encoding();

            //encrypt the password and store it in the hashedBytes byte array
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(model.Password));

            var outputIdParam = new SqlParameter("@userexists", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                using (var conn = new SqlConnection(
                           System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {

                    // Create parameter with Direction as Output (and correct name and type)

                    //sql command to add the user and password to the database
                    var cmd = new SqlCommand("proc_CreateNewUser", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //add parameters to our sql query
                    cmd.Parameters.AddWithValue("@username", model.UserName);

                    cmd.Parameters.AddWithValue("@email", model.Email);
                    cmd.Parameters.AddWithValue("@firstname", model.FirstName);
                    cmd.Parameters.AddWithValue("@lastname", model.LastName);
                    cmd.Parameters.AddWithValue("@phone", model.Phone);

                    cmd.Parameters.AddWithValue("@country", model.Country);
                    cmd.Parameters.AddWithValue("@broker", model.CurrentBroker);
                    cmd.Parameters.AddWithValue("@organisation", model.Organisation ?? "None");
                    cmd.Parameters.AddWithValue("@password", hexString);
                    cmd.Parameters.AddWithValue("@passwordSalt", saltString);

                    cmd.Parameters.Add(outputIdParam);

                    //open the connection
                    conn.Open();
                    //send the sql query to insert the data to our Users table
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            var id = outputIdParam.Value.ToString();

            return true;
        }


        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public void LogQuery(String username, String query, bool answered)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];

            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_LogQuery", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Query", query);
                    sqlCommand.Parameters.AddWithValue("@Answered", answered);
                    sqlCommand.Parameters.AddWithValue("@Username", username);

                    int affectedRows = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }
        }

        public void UnsubscribeQuery(String username, String query)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];

            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_UnsubscribeToQuery", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@username", username);
                    sqlCommand.Parameters.AddWithValue("@query", query);                    

                    int affectedRows = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }
        }

        public void SubscribeToQuery(String username, String query)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];

            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_SubscribeToQuery", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Query", query);
                    sqlCommand.Parameters.AddWithValue("@Username", username);
                    

                    int affectedRows = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }
        }

        public bool Login(String username, String password, bool rememberMe)
        {
            try
            {
                using (var conn = new SqlConnection(
                    System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"].ToString()))
                {
                    var cmd = new SqlCommand("proc_GetUser", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@username", username);

                    conn.Open();

                    var sqlReader = cmd.ExecuteReader();

                    while (sqlReader.Read())
                    {
                        String userId = sqlReader["UserId"].ToString();
                        String usernameT = sqlReader["Username"].ToString();
                        String passwordResult = sqlReader["Password"].ToString();
                        String passwordSalt = sqlReader["PasswordSalt"].ToString();


                        var salt = System.Text.Encoding.UTF8.GetBytes(passwordSalt);
                        var passwordItem = System.Text.Encoding.UTF8.GetBytes(password);

                        var hmacMD5 = new HMACMD5(salt);
                        var saltedHash = hmacMD5.ComputeHash(passwordItem);

                        string hexString = DataModel.ToHex(saltedHash, false);

                        if (passwordResult == hexString)
                        {
                          
                            //FormsAuthentication.SetAuthCookie(username, rememberMe);
                            return true;
                        }
                        else
                        {
                            throw new MembershipCreateUserException();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return false;
        }     
    }
}