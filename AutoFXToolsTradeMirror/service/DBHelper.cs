using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TraceSourceLogger;

namespace Microsoft.Samples.NetTcp
{
    public class DBHelper
    {
        private static readonly Type OType = typeof(DBHelper);

        private const string QueryUsers = "SELECT * FROM users WHERE 1=1 ";

        private List<User> _autoFXUsers;

        public List<User> AutoFXUsers
        {
            get { return this._autoFXUsers; }
            set { this._autoFXUsers = value; }
        }

        private ConnectionManager _connectionManager = null;

        /// <summary>
        /// Augmented Constructor
        /// </summary>
        public DBHelper(ConnectionManager connectionManager)
        {
            this._connectionManager = connectionManager;
            this._autoFXUsers=new List<User>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<User> BuildUsersList()
        {
            try
            {
                MySqlConnection connection = this._connectionManager.Connect();
                MySqlCommand selectDistinctSenders = new MySqlCommand(QueryUsers, connection);
                MySqlDataReader reader = selectDistinctSenders.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader.GetString("id");
                    string email = String.IsNullOrEmpty(reader.GetString("email")) ? "email@default.com" : reader.GetString("email");
                    string role = String.IsNullOrEmpty(reader.GetString("role")) ? "user" : reader.GetString("role");
                    string status = String.IsNullOrEmpty(reader.GetString("status")) ? "0" : reader.GetString("status");
                    string accountNumber = reader.GetString("account_number");
                    string keyString = reader.GetString("keystring");
                    string created = String.IsNullOrEmpty(reader.GetString("created")) ? "1/1/1990" : reader.GetString("created");
                    string modified = String.IsNullOrEmpty(reader.GetString("modified")) ? "1/1/2000" : reader.GetString("modified");

                    User newUser = new User(Convert.ToInt32(id), email, role, Convert.ToBoolean(status),
                                            Convert.ToInt32(accountNumber), keyString, Convert.ToDateTime(created),
                                            Convert.ToDateTime(modified));

                    this._autoFXUsers.Add(newUser);
                    Logger.Info("New User Added = " + newUser, OType.FullName, "BuildUsersList");
                }
                reader.Close();
                this._connectionManager.Disconnect();
                this._autoFXUsers.Sort();
                return this._autoFXUsers;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "BuildUsersList");
                return null;
            }
            
        }

        /// <summary>
        /// Parses and inserts the given message into the DB
        /// </summary>
        /// <param name="message"></param>
        public void ParseAndInsertData(string message)
        {
            try
            {
                MySqlConnection connection = _connectionManager.Connect();
                MySqlCommand dataInsertion = new MySqlCommand {Connection = connection};

                string[] tempArray = message.Split(',');

                if (tempArray.Length == 4)
                {
                    string strategy = tempArray[1].Trim();
                    string signal = tempArray[2].Trim();
                    DateTime processingTime = Convert.ToDateTime(tempArray[3].Trim());
                    //string processingTime = Convert.ToDateTime(tempArray[3].Trim()).ToString("yyyy-MM-dd hh:mm:ss");

                    Logger.Debug(
                        "About to execute query = " + "INSERT INTO signals(strategy, signal, processing_time) " +
                        "values('" + strategy + "'," + signal + ",'" + processingTime.ToString("yyyy-MM-dd hh:mm:ss") + ")", OType.FullName,
                        "ParseAndInsertData");

                    string query = "INSERT INTO signals(strategy, signal, processing_time) " +
                                   "values(@strategy,@signal,@processing_time)";

                    //temp.Close();
                    dataInsertion.CommandText = query;

                    dataInsertion.Parameters.AddWithValue("@strategy", strategy);
                    dataInsertion.Parameters.AddWithValue("@signal", signal);
                    dataInsertion.Parameters.AddWithValue("@processing_time", processingTime.ToString("yyyy-MM-dd hh:mm:ss"));

                    Logger.Debug("Number of Rows Affected = " + dataInsertion.ExecuteNonQuery(), OType.FullName, "ParseAndInsertData");
                }
                else
                {
                    Logger.Debug("Invalid Message = " + message, OType.FullName, "ParseAndInsertData");
                }
                this._connectionManager.Disconnect();
            }

            catch (Exception ex)
            {
                Logger.Error("Exception while parsing or inserting message. Exception = " + ex, OType.FullName, "ParseAndInsertData");
            }
        }
    }
}