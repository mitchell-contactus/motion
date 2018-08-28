using System;
using System.Text;
using MySql.Data.MySqlClient;

namespace Motion.Database
{
    public abstract class DataBase
    {
        protected static readonly string ConnectionStringReadOnly;
        protected static readonly string ConnectionStringReadWrite;

        static DataBase()
        {
            ConnectionStringReadOnly = string.Format("Server={0}; database={1}; UID={2}; password='{3}';Min Pool Size=25;Max Pool Size=100;Pooling=True;charset=utf8mb4; convert zero datetime=True; default command timeout=20;SslMode=none;",
                Config.Get("mysql_host"),
                Config.Get("mysql_db"),
                Config.Get("mysql_user"),
                Config.Get("mysql_pass"));

            ConnectionStringReadWrite = string.Format("Server={0}; database={1}; UID={2}; password='{3}';Min Pool Size=25;Max Pool Size=100;Pooling=True;charset=utf8mb4; convert zero datetime=True; default command timeout=20;SslMode=none;",
                Config.Get("mysql_host"),
                Config.Get("mysql_db"),
                Config.Get("mysql_user"),
                Config.Get("mysql_pass"));
        }

        protected MySqlDataReader Select(String queryString, params object[] parameters) {
            var query = String.Format(queryString, parameters);
            return MySqlHelper.ExecuteReader(ConnectionStringReadOnly, query);
        }

        protected void Insert(String queryString, params object[] parameters) {
            var query = String.Format(queryString, parameters);
            MySqlHelper.ExecuteNonQuery(ConnectionStringReadWrite, query);
        }

        /// <summary>
        ///     Escapes the string for database use
        /// </summary>
        protected string E(string s) {
            return MySqlHelper.EscapeString(s);
        }

        protected string GetSha1(string input)
        {
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha1.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
