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

namespace RemoteAdministrator
{
    public class User : IComparable
    {
        #region Private Members

        private int _id;
        private string _email;
        private string _role;
        private bool _status;
        private string _accountNumber;
        private string _keyString;
        private DateTime _created;
        private DateTime _modified;
        private bool _sendNotifications;
        private string _alternativeEmail;

        #endregion

        #region PublicMembers

        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Email
        {
            get { return this._email; }
            set { this._email = value; }
        }

        public string Role
        {
            get { return this._role; }
            set { this._role = value; }
        }

        public string Status
        {
            get
            {
                if (this._status == true)
                {
                    return "Active";
                }
                else
                {
                    return "Revoked";
                }
            }
        }

        public string AccountNumber
        {
            get { return this._accountNumber; }
            set { this._accountNumber = value; }
        }

        public string KeyString
        {
            get { return this._keyString; }
            set { this._keyString = value; }
        }

        public DateTime Created
        {
            get { return this._created; }
            set { this._created = value; }
        }

        public DateTime Modified
        {
            get { return this._modified; }
            set { this._modified = value; }
        }

        public string SendNotifications
        {
            get
            {
                if (this._sendNotifications == true)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }

        public string AlternativeEmail
        {
            get { return this._alternativeEmail; }
            set { this._alternativeEmail = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public User()
        {
            this._accountNumber = "";
            this._created = DateTime.MinValue;
            this._email = "email@default.com";
            this._id = 0;
            this._keyString = "default";
            this._modified = DateTime.MaxValue;
            this._role = "user";
            this._status = false;
            this._sendNotifications = false;
            this._alternativeEmail = "alternative_email@default.com";
        }

        /// <summary>
        /// Basic Augmented Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountNumber"></param>
        /// <param name="keyString"></param>
        public User(int id, string accountNumber, string keyString)
        {
            this._accountNumber = accountNumber;
            this._created = DateTime.MinValue;
            this._email = "email@default.com";
            this._id = id;
            this._keyString = keyString;
            this._modified = DateTime.MaxValue;
            this._role = "user";
            this._status = true;
            this._sendNotifications = false;
            this._alternativeEmail = "alternative_email@default.com";
        }

        /// <summary>
        /// Complete Augmented Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="accountNumber"></param>
        /// <param name="keyString"></param>
        /// <param name="created"></param>
        /// <param name="modified"></param>
        public User(int id, string email, string role, bool status, string accountNumber, string keyString, DateTime created, DateTime modified)
        {
            this._accountNumber = accountNumber;
            this._created = created;
            this._email = email.ToLower();
            this._id = id;
            this._keyString = keyString;
            this._modified = modified;
            this._role = role;
            this._status = status;
            this._sendNotifications = false;
            this._alternativeEmail = "alternative_email@default.com";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="accountNumber"></param>
        /// <param name="keyString"></param>
        /// <param name="created"></param>
        /// <param name="modified"></param>
        /// <param name="sendNotifications"></param>
        /// <param name="alternativeEmail"></param>
        public User(int id, string email, string role, bool status, string accountNumber, string keyString, DateTime created, DateTime modified, bool sendNotifications, string alternativeEmail)
        {
            this._accountNumber = accountNumber;
            this._created = created;
            this._email = email.ToLower();
            this._id = id;
            this._keyString = keyString;
            this._modified = modified;
            this._role = role;
            this._status = status;
            this._sendNotifications = sendNotifications;
            this._alternativeEmail = alternativeEmail;
        }

        #endregion

        public override string ToString()
        {
            return "Account Number = " + AccountNumber + " | Date Created = " + Created + " | Email ID = " + Email +
                   " | ID = " + ID + " | Key String = " + KeyString + " | Modified Date = " + Modified + " | Role = " +
                   Role + " | Status = " + Status + " | Send Notifications = " + SendNotifications +
                   " | Alternative Email = " + AlternativeEmail;
        }

        public int CompareTo(object obj)
        {
            try
            {
                User user2 = (User)obj;
                if (user2.AccountNumber == this.AccountNumber)
                {
                    if (this.KeyString == user2.KeyString)
                    {
                        if (this.Status == "Active")
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (Convert.ToInt32(user2.AccountNumber) < Convert.ToInt32(this.AccountNumber))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
