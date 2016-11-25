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
using System.Collections.Generic;
using System.Linq;
using TraceSourceLogger;
using UpDownSingnalsServer.Models;

namespace UpDownSingnalsServer.Utility
{
    public static class SearchHelper
    {
        private static readonly Type OType = typeof(SearchHelper);

        public static Action<User> ClientSubscribed;
        public static Action<User> ClientUnSubscribed;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<User> GetActiveUsers(List<User> UsersLists)
        {
            try
            {
                var activeUsers = UsersLists.Where(x => x.Status == "Active").ToList();
                return activeUsers;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetActiveUsers");
                return null;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<User> GetRevokedUsers(List<User> UsersLists)
        {
            try
            {
                var revokedUsers = UsersLists.Where(x => x.Status == "Revoked").ToList();
                return revokedUsers;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetRevokedUsers");
                return null;
            }
        }

        public static List<User> SearchUser(string searchTermType, string searchTerm, string searchFilter, List<User> UsersLists)
        {
            try
            {
                var searchedUsers = new List<User>();
                if (!String.IsNullOrEmpty(searchTerm))
                {
                    if (searchTermType == "Account ID")
                    {
                        if (searchFilter == "All")
                        {
                            searchedUsers =
                                UsersLists.Where(x => x.AccountNumber == searchTerm).ToList();
                        }
                        else if (searchFilter == "Active" || searchFilter == "Revoked")
                        {
                            searchedUsers =
                                UsersLists.Where(
                                    x => (x.AccountNumber == searchTerm) && (x.Status == searchFilter)).
                                    ToList();
                        }
                        else
                        {
                            searchedUsers = UsersLists;
                        }

                    }
                    else if (searchTermType == "Key String")
                    {
                        if (searchFilter == "All")
                        {
                            searchedUsers =
                                UsersLists.Where(x => x.KeyString == searchTerm).ToList();
                        }
                        else if (searchFilter == "Active" || searchFilter == "Revoked")
                        {
                            searchedUsers =
                                UsersLists.Where(
                                    x => (x.KeyString == searchTerm) && (x.Status == searchFilter)).
                                    ToList();
                        }
                        else
                        {
                            searchedUsers = UsersLists;
                        }
                    }
                    else if (searchTermType == "Email Address")
                    {
                        if (searchFilter == "All")
                        {
                            searchedUsers =
                                UsersLists.Where(x => x.Email.Contains(searchTerm)).ToList();
                        }
                        else if (searchFilter == "Active" || searchFilter == "Revoked")
                        {
                            searchedUsers =
                                UsersLists.Where(
                                    x => (x.Email.Contains(searchTerm)) && (x.Status == searchFilter)).
                                    ToList();
                        }
                        else
                        {
                            searchedUsers = UsersLists;
                        }
                    }
                    else
                    {
                        Logger.Info("Insvalid search term type = " + searchTermType, OType.FullName, "SearchUser");
                    }
                }
                else
                {
                    if (searchFilter == "Active" || searchFilter == "Revoked")
                    {
                        searchedUsers =
                            UsersLists.Where(
                                x => (x.Status == searchFilter)).
                                ToList();
                    }
                    else
                    {
                        searchedUsers = UsersLists;
                    }
                }
                return searchedUsers;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SearchUser");
                return new List<User>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="dbHelper"> </param>
        /// <returns></returns>
        public static bool AuthenticateUserCredentials(string userName, DBHelper dbHelper)
        {
            try
            {
                List<User> users = new List<User>();
                users = dbHelper.BuildUsersList();

                User testUser = new User(Convert.ToInt32(userName), userName);
                if (users.BinarySearch(testUser) > -1)
                {
                    if(ClientSubscribed != null)
                    {
                        ClientSubscribed(testUser);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "AuthenticateUserCredentials");
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="dbHelper"> </param>
        /// <returns></returns>
        public static bool UnAuthenticateUserCredentials(string userName, DBHelper dbHelper)
        {
            try
            {
                List<User> users = new List<User>();
                users = dbHelper.BuildUsersList();

                User testUser = new User(Convert.ToInt32(userName), userName);
                if (users.BinarySearch(testUser) > -1)
                {
                    if(ClientUnSubscribed != null)
                    {
                        ClientUnSubscribed(testUser);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "UnAuthenticateUserCredentials");
                return false;
            }

        }
    }
}
