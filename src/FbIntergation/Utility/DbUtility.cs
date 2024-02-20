using Microsoft.Extensions.Options;
using NHLytics.Models;
using System.Data;
using System.Data.SqlClient;

namespace NHlytics.Utility
{
    public class DbUtility
    {
        private readonly ConnectionStrings _connectionStrings;
        public DbUtility(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings.Value;
        }

        public void SaveToken(FbModel model)
        {
            Random rnd = new Random();
            // string token = GetTokenByUserId(model.Email) ?? string.Empty;

            DataTable pageData = new DataTable();
            pageData.Columns.Add("page_id", typeof(string));
            pageData.Columns.Add("long_lived_access_token", typeof(string));
            pageData.Columns.Add("name", typeof(string));
            foreach (var page in model.Pages.Data)
            {
                pageData.Rows.Add(page.Id, page.Token, page.Name);
            }
          
            using (SqlConnection con = new SqlConnection(_connectionStrings.DefaultConnection))
            {
                using (SqlCommand command = new SqlCommand("[Facebook].[InsertUserAndPage]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@user_id", rnd.Next(1, 10000));
                    command.Parameters.AddWithValue("@app_scoped_user_id", model.AppScopedUserId);
                    command.Parameters.AddWithValue("@access_token", model.AccessToken);
                    command.Parameters.AddWithValue("@expires_at", model.Expires_at);
                    command.Parameters.AddWithValue("@email", model.Email);
                    command.Parameters.AddWithValue("@first_name", model.FirstName);
                    command.Parameters.AddWithValue("@last_name", model.LastName);
                    SqlParameter tvpParam = command.Parameters.AddWithValue("@pages", pageData);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "[Facebook].[PageType]";
                   
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                }
            }

        }

        public string GetTokenByUserId(string email)
        {
            string access_token = string.Empty;
            using (SqlConnection connection = new SqlConnection(_connectionStrings.DefaultConnection))
            {
                string query = "SELECT access_token FROM [Facebook].[User] where email = @email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        access_token = reader["access_token"].ToString();

                    }
                }
                connection.Close();
            }
            return access_token;
        }

        public void DeleteToken(string email)
        {
            using (var sc = new SqlConnection(_connectionStrings.DefaultConnection))
            using (var cmd = sc.CreateCommand())
            {
                sc.Open();
                cmd.CommandText = "DELETE FROM [Facebook].[User] WHERE email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();
            }
        }
    }
}