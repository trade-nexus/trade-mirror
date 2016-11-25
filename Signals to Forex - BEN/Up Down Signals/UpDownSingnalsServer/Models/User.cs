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

namespace UpDownSingnalsServer.Models
{
    public class User : IComparable
    {
        private bool _status;

        #region PublicMembers

        public int ID { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

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

        public string AccountNumber { get; set; }

        public string KeyString { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public User()
        {
            this.AccountNumber = "";
            this.Created = DateTime.MinValue;
            this.Email = "email@default.com";
            this.ID = 0;
            this.KeyString = "default";
            this.Modified = DateTime.MaxValue;
            this.Role = "user";
            this._status = false;
        }

        /// <summary>
        /// Basic Augmented Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountNumber"></param>
        public User(int id, string accountNumber)
        {
            this.AccountNumber = accountNumber;
            this.Created = DateTime.MinValue;
            this.Email = "email@default.com";
            this.ID = id;
            this.KeyString = "default";
            this.Modified = DateTime.MaxValue;
            this.Role = "user";
            this._status = true;
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
            this.AccountNumber = accountNumber;
            this.Created = created;
            this.Email = email.ToLower();
            this.ID = id;
            this.KeyString = keyString;
            this.Modified = modified;
            this.Role = role;
            this._status = status;
        }

        #endregion

        public override string ToString()
        {
            return "Account Number = " + AccountNumber + " | Date Created = " + Created + " | Email ID = " + Email +
                   " | ID = " + ID + " | Key String = " + KeyString + " | Modified Date = " + Modified + " | Role = " +
                   Role + " | Status = " + Status;
        }

        public int CompareTo(object obj)
        {
            try
            {
                User user2 = (User)obj;
                if (user2.AccountNumber == this.AccountNumber)
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
