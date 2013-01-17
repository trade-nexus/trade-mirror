using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public class DBHelper
    {
        private static readonly Type OType = typeof(DBHelper);

        private const string QueryUsers = "SELECT * FROM users WHERE 1=1 ";
        private const string QuerySuffixes = "SELECT * FROM suffixes WHERE 1=1 ";

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
                this._autoFXUsers.Clear();
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
                    string notificationStatus = String.IsNullOrEmpty(reader.GetString("send_notfications")) ? "0" : reader.GetString("send_notfications");
                    string alternativeEmail = String.IsNullOrEmpty(reader.GetString("alternate_email")) ? "email@default.com" : reader.GetString("alternate_email");

                    User newUser = new User(Convert.ToInt32(id), email, role, Convert.ToBoolean(status),
                                            Convert.ToInt32(accountNumber), keyString, Convert.ToDateTime(created),
                                            Convert.ToDateTime(modified), Convert.ToBoolean(notificationStatus), alternativeEmail);

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
                    string processingTime = tempArray[3].TrimEnd(';');
                    //DateTime processingTime = Convert.ToDateTime(time.ToString("yyyyMMddHHmmss"));
                    //string processingTime = Convert.ToDateTime(tempArray[3].TrimEnd(';')).ToString("yyyyMMddHHmmss");
                    //string processingTime = tempArray[3].Trim();

                    Logger.Debug(
                        "About to execute query = " + "INSERT INTO signals(strategy, signal, processing_time) " +
                        "values(`" + strategy + "`,`" + signal + "`," + processingTime + ")", OType.FullName,
                        "ParseAndInsertData");

                    const string query = "INSERT INTO signals(`strategy`, `signal`, `processing time`) " +
                                         "values(@strategy,@signal,@processing_time)";

                    //temp.Close();
                    dataInsertion.CommandText = query;

                    dataInsertion.Parameters.AddWithValue("@strategy", strategy);
                    dataInsertion.Parameters.AddWithValue("@signal", signal);
                    dataInsertion.Parameters.AddWithValue("@processing_time", processingTime);

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

        /// <summary>
        /// 
        /// </summary>
        public void AddNewUser(string email, int account, string key, string status, string notificationStatus, string alternateEmail)
        {
            try
            {
                int dbStatus = status == "Active" ? 1 : 0;
                int dbNotificationStatus = notificationStatus == "Yes" ? 1 : 0;

                MySqlConnection connection = _connectionManager.Connect();
                MySqlCommand dataInsertion = new MySqlCommand { Connection = connection };

                string creationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                string modificationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                const string query = "INSERT INTO users(`email`,`account_number`,`keystring`,`status`, `created`, `modified`, `send_notfications`, `alternate_email`)" +
                                     " VALUES (@email, @account, @key, @status, @created, @modified, @notificationStatus, @alternateEmail)";

                dataInsertion.CommandText = query;

                dataInsertion.Parameters.AddWithValue("@email", email);
                dataInsertion.Parameters.AddWithValue("@account", account);
                dataInsertion.Parameters.AddWithValue("@key", key);
                dataInsertion.Parameters.AddWithValue("@status", dbStatus);
                dataInsertion.Parameters.AddWithValue("@created", creationTime);
                dataInsertion.Parameters.AddWithValue("@modified", modificationTime);
                dataInsertion.Parameters.AddWithValue("@notificationStatus", dbNotificationStatus);
                dataInsertion.Parameters.AddWithValue("@alternateEmail", alternateEmail);

                Logger.Debug("Number of Rows Affected = " + dataInsertion.ExecuteNonQuery(), OType.FullName, "AddUser");
                _connectionManager.Disconnect();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "AddUser");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EditUser(int id, string email, int account, string key, string status, string notificationStatus, string alternateEmail)
        {
            try
            {
                int dbStatus = status == "Active" ? 1 : 0;
                int dbNotificationStatus = notificationStatus == "Yes" ? 1 : 0;

                MySqlConnection connection = _connectionManager.Connect();
                MySqlCommand dataInsertion = new MySqlCommand { Connection = connection };

                string modificationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                const string query = "UPDATE users SET `email`=@email, `account_number`=@account,`keystring`=@key, `status`=@status, `modified`=@modified, " +
                                     "`send_notfications`=@notificationStatus, `alternate_email`=@alternateEmail WHERE `id`=@id";

                dataInsertion.CommandText = query;

                dataInsertion.Parameters.AddWithValue("@email", email);
                dataInsertion.Parameters.AddWithValue("@account", account);
                dataInsertion.Parameters.AddWithValue("@key", key);
                dataInsertion.Parameters.AddWithValue("@status", dbStatus);
                dataInsertion.Parameters.AddWithValue("@id", id);
                dataInsertion.Parameters.AddWithValue("@modified", modificationTime);
                dataInsertion.Parameters.AddWithValue("@notificationStatus", dbNotificationStatus);
                dataInsertion.Parameters.AddWithValue("@alternateEmail", alternateEmail);

                Logger.Debug("Number of Rows Affected = " + dataInsertion.ExecuteNonQuery(), OType.FullName, "EditUser");
                _connectionManager.Disconnect();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "EditUser");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSuffixes()
        {
            try
            {
                MySqlConnection connection = this._connectionManager.Connect();
                MySqlCommand selectSuffixes = new MySqlCommand(QuerySuffixes, connection);
                MySqlDataReader reader = selectSuffixes.ExecuteReader();

                string suffixes = string.Empty;

                while (reader.Read())
                {
                    string newSuffix = String.IsNullOrEmpty(reader.GetString("suffix"))
                                           ? ""
                                           : reader.GetString("suffix");
                    if(suffixes == "")
                    {
                        suffixes = newSuffix;
                    }
                    else
                    {
                        suffixes = suffixes + "," + newSuffix;
                    }

                    Logger.Info("New Suffix Added = " + newSuffix, OType.FullName, "GetSuffixes");
                }
                reader.Close();
                this._connectionManager.Disconnect();

                return suffixes;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetSuffixes");
                return null;
            }
        }
    }
}