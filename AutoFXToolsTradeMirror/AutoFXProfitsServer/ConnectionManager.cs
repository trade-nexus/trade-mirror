/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Trade Mirror provides an infrastructure for low latency trade copying
* services from master to child traders, and also trader to different
* channels including social media. It is a highly customizable solution
* with low-latency signal transmission capabilities. The tool can copy trades
* from sender and publish them to all subscribed receiver’s in real time
* across a local network or the internet. Trade Mirror is built using
* languages and frameworks that include C#, C++, WPF, WCF, Socket Programming,
* MySQL, NUnit and MT4 and MT5 MetaTrader platforms.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public class ConnectionManager
    {
        private static readonly Type OType = typeof(ConnectionManager);

        //private const string ConnectionString = "SERVER=localhost;DATABASE=autofxproduction;UID=root;PASSWORD=rootpassword";
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["autofxproduction"].ConnectionString;

        private readonly MySqlConnection _connection;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionManager()
        {
            this._connection = new MySqlConnection {ConnectionString = _connectionString};
        }
        
        /// <summary>
        /// Open a connection to DB
        /// </summary>
        /// <returns></returns>
        public MySqlConnection Connect()
        {
            try
            {
                this._connection.Open();
                Logger.Debug("Connected to database", OType.FullName, "Connect");
                return this._connection;
            }

            catch (Exception exception)
            {
                Logger.Error("Exception while opening connection to Databse. Exception = " + exception, OType.FullName, "Connect");
                return null;
            }
        }

        /// <summary>
        /// Close connection to DB
        /// </summary>
        public void Disconnect()
        {
            try
            {
                this._connection.Close();
                Logger.Debug("Dis-Connected from database", OType.FullName, "Disconnect");
            }
            catch (Exception exception)
            {
                Logger.Error("Exception while closing connection to Databse. Exception = " + exception, OType.FullName, "Disconnect");
            }
        }
    }
}
