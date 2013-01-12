using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public static class SearchHelper
    {
        private static readonly Type OType = typeof(AutoFXToolsServerShellViewModel);

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
                                UsersLists.Where(x => x.AccountNumber == Convert.ToInt32(searchTerm)).ToList();
                        }
                        else if (searchFilter == "Active" || searchFilter == "Revoked")
                        {
                            searchedUsers =
                                UsersLists.Where(
                                    x => (x.AccountNumber == Convert.ToInt32(searchTerm)) && (x.Status == searchFilter)).
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

        public static string GetActiveUserAlternateAddresses(List<User> UsersLists)
        {
            try
            {
                List<User> users = new List<User>();
                users = GetActiveUsers(UsersLists);
                string userAddresses = users.Aggregate(String.Empty, (current, user) => current + ";" + user.AlternativeEmail);

                return userAddresses;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetActiveUserAddresses");
                return null;
            }
        }

        public static string GetReceipientsFromType(List<User> UsersLists, string receipientType)
        {
            try
            {
                List<User> users = new List<User>();

                if(receipientType == "All")
                {
                    users = UsersLists;
                }
                else if(receipientType == "Active")
                {
                    users = GetActiveUsers(UsersLists);
                }
                else if (receipientType == "Revoked")
                {
                    users = GetRevokedUsers(UsersLists);
                }
                
                string userAddresses = users.Aggregate(String.Empty, (current, user) => current + ";" + user.AlternativeEmail);

                return userAddresses;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetReceipientsFromType");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="accountID"></param>
        /// <param name="dbHelper"> </param>
        /// <returns></returns>
        public static bool AuthenticateUserCredentials(string userName, string password, int accountID, DBHelper dbHelper)
        {
            try
            {
                List<User> users = new List<User>();
                users = dbHelper.BuildUsersList();

                User testUser = new User(Convert.ToInt32(userName), Convert.ToInt32(userName), password);
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
        /// <param name="accountID"></param>
        /// <param name="dbHelper"> </param>
        /// <returns></returns>
        public static bool UnAuthenticateUserCredentials(string userName, string password, int accountID, DBHelper dbHelper)
        {
            try
            {
                List<User> users = new List<User>();
                users = dbHelper.BuildUsersList();

                User testUser = new User(Convert.ToInt32(userName), Convert.ToInt32(userName), password);
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
