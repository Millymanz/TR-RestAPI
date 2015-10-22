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

            //saved queries
            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_ViewUserSavedQueries", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@username", username);

                    SqlDataReader rdr = sqlCommand.ExecuteReader();
                    while (rdr.Read())
                    {
                        var queryCard = new QueryCard();

                        queryCard.QueryID = rdr["QueryLogId"].ToString();
                        queryCard.Query = rdr["Query"].ToString();

                        userProfileConfig.SavedQueries.Add(queryCard);
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

        //public bool CreateUserAndAccount(RegisterModel model)
        //{
        //    String saltString = "Generatedinordertochangetheworldweknow934";
        //    var salt = System.Text.Encoding.UTF8.GetBytes(saltString);
        //    var password = System.Text.Encoding.UTF8.GetBytes(model.Password);

        //    var hmacMD5 = new HMACMD5(salt);
        //    var saltedHash = hmacMD5.ComputeHash(password);

        //    string hexString = DataModel.ToHex(saltedHash, false);

        //    //create the MD5CryptoServiceProvider object we will use to encrypt the password
        //    var md5Hasher = new MD5CryptoServiceProvider();
        //    //create an array of bytes we will use to store the encrypted password
        //    Byte[] hashedBytes;

        //    //Create a UTF8Encoding object we will use to convert our password string to a byte array
        //    var encoder = new UTF8Encoding();

        //    //encrypt the password and store it in the hashedBytes byte array
        //    hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(model.Password));

        //    var outputIdParam = new SqlParameter("@userexists", SqlDbType.Bit)
        //    {
        //        Direction = ParameterDirection.Output
        //    };

        //    try
        //    {
        //        using (var conn = new SqlConnection(
        //                   System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
        //        {

        //            // Create parameter with Direction as Output (and correct name and type)

        //            //sql command to add the user and password to the database
        //            var cmd = new SqlCommand("proc_CreateNewUser", conn);
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            //add parameters to our sql query
        //            cmd.Parameters.AddWithValue("@username", model.UserName);

        //            cmd.Parameters.AddWithValue("@email", model.Email);
        //            cmd.Parameters.AddWithValue("@firstname", model.FirstName);
        //            cmd.Parameters.AddWithValue("@lastname", model.LastName);
        //            cmd.Parameters.AddWithValue("@phone", model.Phone);

        //            cmd.Parameters.AddWithValue("@country", model.Country);
        //            cmd.Parameters.AddWithValue("@broker", model.CurrentBroker);
        //            cmd.Parameters.AddWithValue("@organisation", model.Organisation ?? "None");
        //            cmd.Parameters.AddWithValue("@password", hexString);
        //            cmd.Parameters.AddWithValue("@passwordSalt", saltString);

        //            cmd.Parameters.Add(outputIdParam);

        //            //open the connection
        //            conn.Open();
        //            //send the sql query to insert the data to our Users table
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //    var id = outputIdParam.Value.ToString();

        //    return true;
        //}


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

        private string PrepTextForLikeQueries(String query)
        {
            query += "%";
            var final = query.Insert(0, "%");

            return final;
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
                    sqlCommand.Parameters.AddWithValue("@query", PrepTextForLikeQueries(query));                    

                    int affectedRows = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }
        }

        public void UnsaveUserQueries(String username, String query)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];

            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_UnsaveUserQueries", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@username", username);
                    sqlCommand.Parameters.AddWithValue("@query", PrepTextForLikeQueries(query));                    

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
                    sqlCommand.Parameters.AddWithValue("@Query", PrepTextForLikeQueries(query));
                    sqlCommand.Parameters.AddWithValue("@Username", username);
                    

                    int affectedRows = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }
        }

        public void SaveUserQuery(String username, String query)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];

            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_SaveQuery", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Query", PrepTextForLikeQueries(query));
                    sqlCommand.Parameters.AddWithValue("@Username", username);


                    int affectedRows = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }
        }

        public bool LoginTokenTest(String username, string currentAccessToken, DateTime startDateTime, DateTime endDateTime)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];

            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_LoginUserTest", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Username", "traderiserapp@traderiser.com");     
                    int affectedRows = sqlCommand.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return false;
        }

        public bool CheckUserLoginStatus(string username, string token)
        {
            //change this code so that in future it only makes one trip to the database
            bool loggedIn = false;
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];
            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_CheckUserLoginStatus", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Username", username);
                    sqlCommand.Parameters.AddWithValue("@Token", token);

                    SqlDataReader rdr = sqlCommand.ExecuteReader();
                    while (rdr.Read())
                    {
                        loggedIn = Convert.ToBoolean(rdr["LoggedIn"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }
            return loggedIn;
        }

        public UserInfo RegisterToken(string username, string token)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];
            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_RegisterToken", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Username", username);
                    sqlCommand.Parameters.AddWithValue("@Token", token);

                    int affected = sqlCommand.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        return GetUserInfo_Username(username);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }
            return null;
        }


        public bool HasTokenExpired(string username, string token)
        {
            DateTime startDateTime = new DateTime();
            DateTime endDateTime = new DateTime();

            bool expired = false;
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];
            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_GetSessionDateTimes", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Username", username);
                    sqlCommand.Parameters.AddWithValue("@Token", token);

                    SqlDataReader rdr = sqlCommand.ExecuteReader();
                    while (rdr.Read())
                    {
                        startDateTime = Convert.ToDateTime(rdr["StartDateTime"].ToString());
                        endDateTime = Convert.ToDateTime(rdr["EndDateTime"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generic Exception:: " + ex.Message);
            }

            if (DateTime.Now >= endDateTime)
            {
                expired = true;
            }

            return expired;
        }


        public bool LoginToken(String username, string currentAccessToken, DateTime startDateTime, DateTime endDateTime)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];

            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_LoginUser", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Username", username);
                    sqlCommand.Parameters.AddWithValue("@CurrentAccessToken", currentAccessToken);
                    sqlCommand.Parameters.AddWithValue("@StartDateTime", startDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sqlCommand.Parameters.AddWithValue("@EndDateTime", endDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    int affectedRows = sqlCommand.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return false;
        }

        public bool LogoutToken(String username, string currentAccessToken)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"];

            try
            {
                using (SqlConnection con = new SqlConnection(settings.ToString()))
                {
                    SqlCommand sqlCommand = new SqlCommand("proc_LogoutUser", con);
                    con.Open();

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Username", username);
                    sqlCommand.Parameters.AddWithValue("@CurrentAccessToken", currentAccessToken);

                    int affectedRows = sqlCommand.ExecuteNonQuery();
                    if (affectedRows > 0)
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return false;
        }

        public UserInfo GetUserInfo_Token(String token)
        {
            var userInfo = new UserInfo();

            try
            {
                using (var conn = new SqlConnection(
                    System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"].ToString()))
                {
                    var cmd = new SqlCommand("proc_GetUser_Token", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@username", token);

                    conn.Open();

                    var sqlReader = cmd.ExecuteReader();

                    while (sqlReader.Read())
                    {

                        String userId = sqlReader["UserId"].ToString();
                        String usernameT = sqlReader["Username"].ToString();
                        String firstName = sqlReader["FirstName"].ToString();
                        String lastName = sqlReader["LastName"].ToString();
                        String email = sqlReader["Email"].ToString();

                        //userInfo.UserId = Convert.ToInt32(userId);
                        userInfo.Username = usernameT;
                        userInfo.FirstName = firstName;
                        userInfo.LastName = lastName;
                        userInfo.Email = email;
                        userInfo.LoginSuccessful = true;

                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return userInfo;
        }

        public UserInfo GetUserInfo_Username(String username)
        {
            var userInfo = new UserInfo();

            try
            {
                using (var conn = new SqlConnection(
                    System.Configuration.ConfigurationManager.ConnectionStrings["UsermanagementConnection"].ToString()))
                {
                    var cmd = new SqlCommand("proc_GetUser_Username", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@username", username);

                    conn.Open();

                    var sqlReader = cmd.ExecuteReader();

                    while (sqlReader.Read())
                    {

                        String userId = sqlReader["UserId"].ToString();
                        String usernameT = sqlReader["Username"].ToString();
                        String firstName = sqlReader["FirstName"].ToString();
                        String lastName = sqlReader["LastName"].ToString();
                        String email = sqlReader["Email"].ToString();

                        //userInfo.UserId = Convert.ToInt32(userId);
                        userInfo.Username = usernameT;
                        userInfo.FirstName = firstName;
                        userInfo.LastName = lastName;
                        userInfo.Email = email;
                        userInfo.LoginSuccessful = true;

                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return userInfo;
        }     


        public UserInfo GetUserInfo(String username, String password, bool rememberMe)
        {
            var userInfo = new UserInfo();

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
                        String firstName = sqlReader["FirstName"].ToString();
                        String lastName = sqlReader["LastName"].ToString();
                        String email = sqlReader["Email"].ToString();

                        //userInfo.UserId = Convert.ToInt32(userId);
                        userInfo.Username = usernameT;
                        userInfo.FirstName = firstName;
                        userInfo.LastName = lastName;
                        userInfo.Email = email;
                        userInfo.LoginSuccessful = true;

                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return userInfo;
        }     


        public UserInfo Login(String username, String password, bool rememberMe)
        {
            var userInfo = new UserInfo();

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
                        String firstName = sqlReader["FirstName"].ToString();
                        String lastName = sqlReader["LastName"].ToString();
                        String email = sqlReader["Email"].ToString();


                        var salt = System.Text.Encoding.UTF8.GetBytes(passwordSalt);
                        var passwordItem = System.Text.Encoding.UTF8.GetBytes(password);

                        var hmacMD5 = new HMACMD5(salt);
                        var saltedHash = hmacMD5.ComputeHash(passwordItem);

                        string hexString = DataModel.ToHex(saltedHash, false);

                        //userInfo.UserId = Convert.ToInt32(userId);
                        userInfo.Username = usernameT;
                        userInfo.FirstName = firstName;
                        userInfo.LastName = lastName;
                        userInfo.Email = email;

                        if (passwordResult == hexString)
                        {
                            userInfo.LoginSuccessful = true;
            
                        }
                        else
                        {
                            userInfo.LoginSuccessful = false;

                            throw new MembershipCreateUserException();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return userInfo;
        }     
    }
}