using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public class SearchHelper
    {
        private static readonly Type OType = typeof(AutoFXToolsServerShellViewModel);
        
        public List<User> UsersLists { get; set; }
        
        public SearchHelper(List<User> systermUsers)
        {
            this.UsersLists = systermUsers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<User> GetActiveUsers()
        {
            try
            {
                var activeUsers = UsersLists.Where(x => x.Status == "Active").ToList();
                return activeUsers;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetActiveUsers");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<User> GetRevokedUsers()
        {
            try
            {
                var revokedUsers = UsersLists.Where(x => x.Status == "Revoked").ToList();
                return revokedUsers;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetRevokedUsers");
                throw;
            }
        }

        public List<User> SearchUser(string searchTermType, string searchTerm, string searchFilter)
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

        public string GetActiveUserAddresses()
        {
            try
            {
                List<User> users = new List<User>();
                users = GetActiveUsers();
                string userAddresses = users.Aggregate(string.Empty, (current, user) => current + ";" + user.Email);

                return userAddresses;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetActiveUserAddresses");
                throw;
            }
        }
    }
}
