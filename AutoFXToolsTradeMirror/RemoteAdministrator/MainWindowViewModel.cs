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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TraceSourceLogger;

namespace RemoteAdministrator
{
    public class MainWindowViewModel : DependencyObject
    {
        private static readonly Type OType = typeof(MainWindowViewModel);

        #region Command Declaration

        public ICommand SearchGoCommand { get; set; }
        public ICommand AddNewUserCommand { get; set; }
        public ICommand EditUserCommand { get; set; }
        public ICommand ExportUserCommand { get; set; }
        public ICommand SaveUserCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SortByIDCommand { get; set; }
        public ICommand SortByAccountIDCommand { get; set; }
        public ICommand SortByKeyStringCommand { get; set; }
        public ICommand SortByEmailCommand { get; set; }
        public ICommand SortByStatusCommand { get; set; }
        public ICommand RevokedUsersCheckedCommand { get; set; }
        public ICommand ActiveUsersCheckedCommand { get; set; }
        public ICommand AllUsersCheckedCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        #endregion

        #region SearchItem

        public static readonly DependencyProperty SearchItemProperty =
            DependencyProperty.Register("SearchItem", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(default(string)));

        public string SearchItem
        {
            get { return (string)GetValue(SearchItemProperty); }
            set { SetValue(SearchItemProperty, value); }
        }

        #endregion

        #region SearchTermsCollection

        public static readonly DependencyProperty SearchTermsCollectionProperty =
            DependencyProperty.Register("SearchTermsCollection", typeof(ObservableCollection<string>), typeof(MainWindowViewModel), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> SearchTermsCollection
        {
            get { return (ObservableCollection<string>)GetValue(SearchTermsCollectionProperty); }
            set { SetValue(SearchTermsCollectionProperty, value); }
        }

        #endregion

        #region SelectedSearchType

        public static readonly DependencyProperty SelectedSearchTypeProperty =
            DependencyProperty.Register("SelectedSearchType", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(default(string)));

        public string SelectedSearchType
        {
            get { return (string)GetValue(SelectedSearchTypeProperty); }
            set { SetValue(SelectedSearchTypeProperty, value); }
        }

        #endregion

        #region IsAllUsersChecked

        public static readonly DependencyProperty IsAllUsersCheckedProperty =
            DependencyProperty.Register("IsAllUsersChecked", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(true));

        public bool IsAllUsersChecked
        {
            get { return (bool)GetValue(IsAllUsersCheckedProperty); }
            set { SetValue(IsAllUsersCheckedProperty, value); }
        }

        #endregion

        #region IsActiveUsersChecked

        public static readonly DependencyProperty IsActiveUsersCheckedProperty =
            DependencyProperty.Register("IsActiveUsersChecked", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(default(bool)));

        public bool IsActiveUsersChecked
        {
            get { return (bool)GetValue(IsActiveUsersCheckedProperty); }
            set { SetValue(IsActiveUsersCheckedProperty, value); }
        }

        #endregion

        #region IsRevokedUsersChecked

        public static readonly DependencyProperty IsRevokedUsersCheckedProperty =
            DependencyProperty.Register("IsRevokedUsersChecked", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(default(bool)));

        public bool IsRevokedUsersChecked
        {
            get { return (bool)GetValue(IsRevokedUsersCheckedProperty); }
            set { SetValue(IsRevokedUsersCheckedProperty, value); }
        }

        #endregion

        #region FilteredUsersCollection

        public static readonly DependencyProperty FilteredUsersCollectionProperty =
            DependencyProperty.Register("FilteredUsersCollection", typeof(ObservableCollection<User>), typeof(MainWindowViewModel), new PropertyMetadata((new ObservableCollection<User>())));

        public ObservableCollection<User> FilteredUsersCollection
        {
            get { return (ObservableCollection<User>)GetValue(FilteredUsersCollectionProperty); }
            set { SetValue(FilteredUsersCollectionProperty, value); }
        }

        #endregion

        #region SelectedUser

        public static readonly DependencyProperty SelectedUserProperty =
            DependencyProperty.Register("SelectedUser", typeof(object), typeof(MainWindowViewModel), new PropertyMetadata(default(object)));

        public object SelectedUser
        {
            get { return (object)GetValue(SelectedUserProperty); }
            set { SetValue(SelectedUserProperty, value); }
        }

        #endregion

        #region NotificationStatusesCollection

        public static readonly DependencyProperty NotificationStatusesCollectionProperty =
            DependencyProperty.Register("NotificationStatusesCollection", typeof(ObservableCollection<string>), typeof(MainWindowViewModel), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> NotificationStatusesCollection
        {
            get { return (ObservableCollection<string>)GetValue(NotificationStatusesCollectionProperty); }
            set { SetValue(NotificationStatusesCollectionProperty, value); }
        }

        #endregion

        #region UserStatusesCollection

        public static readonly DependencyProperty UserStatusesCollectionProperty =
            DependencyProperty.Register("UserStatusesCollection", typeof(ObservableCollection<string>), typeof(MainWindowViewModel), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> UserStatusesCollection
        {
            get { return (ObservableCollection<string>)GetValue(UserStatusesCollectionProperty); }
            set { SetValue(UserStatusesCollectionProperty, value); }
        }

        #endregion

        #region ID

        public int ID { get; set; }

        #endregion

        #region Email

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(default(string)));

        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        #endregion

        #region Account

        public static readonly DependencyProperty AccountProperty =
            DependencyProperty.Register("Account", typeof (string), typeof (MainWindowViewModel), new PropertyMetadata(default(string)));

        public string Account
        {
            get { return (string) GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion

        #region Key

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(default(string)));

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        #endregion

        #region SelectedStatus

        public static readonly DependencyProperty SelectedStatusProperty =
            DependencyProperty.Register("SelectedStatus", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(default(string)));

        public string SelectedStatus
        {
            get { return (string)GetValue(SelectedStatusProperty); }
            set { SetValue(SelectedStatusProperty, value); }
        }

        #endregion

        #region AlternateEmail

        public static readonly DependencyProperty AlternativeEmailProperty =
            DependencyProperty.Register("AlternativeEmail", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(""));

        public string AlternativeEmail
        {
            get { return (string)GetValue(AlternativeEmailProperty); }
            set { SetValue(AlternativeEmailProperty, value); }
        }

        #endregion

        #region SelectedNotificationMode

        public static readonly DependencyProperty SelectedNotificationModeProperty =
            DependencyProperty.Register("SelectedNotificationMode", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(default(string)));

        public string SelectedNotificationMode
        {
            get { return (string)GetValue(SelectedNotificationModeProperty); }
            set { SetValue(SelectedNotificationModeProperty, value); }
        }

        #endregion

        public List<User> AutoFXUsers { get; set; }

        private DBHelper _helper = null;
        
        private bool _editStatus = false;
        private UserWindow _userWindow;

        private static Timer _refreshUsersTimer;
        private const int RefreshPeriod = 150;

        public bool EditStatus
        {
            get { return this._editStatus; }
            set { this._editStatus = value; }
        }

        /// <summary>
        /// Holds reference to UI dispatcher
        /// </summary>
        private readonly Dispatcher _currentDispatcher;

        private DeleteUserConfirmation _deleteUserConfirmationWindow;

        private int _sortingState = 0;

        public MainWindowViewModel()
        {
            #region Command Initialization

            this.SearchGoCommand = new SearchGoCommand(this);
            this.AddNewUserCommand = new AddNewUserCommand(this);
            this.EditUserCommand = new EditUserCommand(this);
            this.ExportUserCommand = new ExportUserCommand(this);
            this.SaveUserCommand = new SaveUserCommand(this);
            this.RefreshCommand = new RefreshCommand(this);
            this.SortByIDCommand = new SortByIDCommand(this);
            this.SortByAccountIDCommand = new SortByAccountIDCommand(this);
            this.SortByKeyStringCommand = new SortByKeyStringCommand(this);
            this.SortByEmailCommand = new SortByEmailCommand(this);
            this.SortByStatusCommand = new SortByStatusCommand(this);
            this.AllUsersCheckedCommand = new AllUsersCheckedCommand(this);
            this.ActiveUsersCheckedCommand = new ActiveUsersCheckedCommand(this);
            this.RevokedUsersCheckedCommand = new RevokedUsersCheckedCommand(this);
            this.DeleteCommand = new DeleteCommand(this);

            #endregion

            this._currentDispatcher = Dispatcher.CurrentDispatcher;

            _refreshUsersTimer = new Timer(RefreshPeriod * 1000);
            _refreshUsersTimer.Elapsed += RefreshUsersTimerElapsed;
            _refreshUsersTimer.AutoReset = true;

            var connectionManager = new ConnectionManager();
            _helper = new DBHelper(connectionManager);
            AutoFXUsers = _helper.BuildUsersList();

            InitializeSearchTermsCollection();
            InitializeFilteredUsersCollection();
            InitializeUserStatusesCollection();
            InitializeNotificationStatusesCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddNewUser()
        {
            _editStatus = false;

            ResetUserInformation();

            _userWindow = new UserWindow(this);
            _userWindow.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        public void EditUser()
        {
            _editStatus = true;
            _userWindow = new UserWindow(this);
            _userWindow.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteUser()
        {
            _deleteUserConfirmationWindow = new DeleteUserConfirmation();
            _deleteUserConfirmationWindow.ShowDialog();

            if (_deleteUserConfirmationWindow.MessageBoxSelection)
            {
                _helper.DeleteUser(ID);
                UpdateUserList();
                ResetUserInformation();
                _deleteUserConfirmationWindow.Close();
            }
            else
            {
                _deleteUserConfirmationWindow.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveUser()
        {
            if (_editStatus)
            {
                _helper.EditUser(ID, Email, Account, Key, SelectedStatus, SelectedNotificationMode, AlternativeEmail);
            }
            else
            {
                if (String.IsNullOrEmpty(AlternativeEmail))
                {
                    AlternativeEmail = Email;
                }
                _helper.AddNewUser(Email, Account, Key, SelectedStatus, SelectedNotificationMode, AlternativeEmail);
            }
            UpdateUserList();
            _userWindow.Close();

            ResetUserInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateSelectedUserDetails()
        {
            try
            {
                var selectedUser = (User)SelectedUser;

                if (selectedUser != null)
                {
                    this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        this.ID = selectedUser.ID;
                        this.Email = selectedUser.Email;
                        this.SelectedStatus = selectedUser.Status;
                        this.Account = selectedUser.AccountNumber;
                        this.Key = selectedUser.KeyString;
                        this.SelectedNotificationMode = selectedUser.SendNotifications;
                        this.AlternativeEmail = selectedUser.AlternativeEmail;
                    }));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "UpdateSelectedUserDetails");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateUserList()
        {
            try
            {
                AutoFXUsers.Clear();
                AutoFXUsers = _helper.BuildUsersList();

                AutoFXUsers = AutoFXUsers.OrderBy(x => x.ID).ToList();
                AutoFXUsers.Reverse();

                FilteredUsersCollection.Clear();
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    foreach (var user in AutoFXUsers)
                    {
                        this.FilteredUsersCollection.Add(user);
                    }
                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "UpdateUserList");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExportUsers()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Export";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = path + "\\user_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (File.Create(path))
                {
                }

                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                foreach (var autoFXUser in AutoFXUsers)
                {
                    streamWriter.WriteLine(autoFXUser.Email);
                    if (autoFXUser.AlternativeEmail != autoFXUser.Email)
                    {
                        streamWriter.WriteLine(autoFXUser.AlternativeEmail);
                    }
                }

                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ExportUsers");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void FreeResources()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void SearchUsers()
        {
            try
            {
                string searchFilter = "All";
                this.FilteredUsersCollection.Clear();

                if (IsAllUsersChecked)
                {
                    searchFilter = "All";
                }
                else if (IsActiveUsersChecked)
                {
                    searchFilter = "Active";
                }
                else if (IsRevokedUsersChecked)
                {
                    searchFilter = "Revoked";
                }

                var searchedUsers = SearchHelper.SearchUser(SelectedSearchType, SearchItem, searchFilter, AutoFXUsers);
                searchedUsers = searchedUsers.OrderBy(x => x.ID).ToList();
                searchedUsers.Reverse();
                foreach (var revokedUser in searchedUsers)
                {
                    this.FilteredUsersCollection.Add(revokedUser);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SearchUsers");
            }
        }

        /// <summary>
        /// Initializes the Search Terms Collection
        /// </summary>
        private void InitializeSearchTermsCollection()
        {
            try
            {
                this.SearchTermsCollection = new ObservableCollection<string>();
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    this.SearchTermsCollection.Add("Account ID");
                    this.SearchTermsCollection.Add("Key String");
                    this.SearchTermsCollection.Add("Email Address");
                }));

            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "InitializeSearchTermsCollection");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeFilteredUsersCollection()
        {
            try
            {
                this.FilteredUsersCollection = new ObservableCollection<User>();
                AutoFXUsers = AutoFXUsers.OrderBy(x => x.ID).ToList();
                AutoFXUsers.Reverse();

                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    foreach (var user in AutoFXUsers)
                    {
                        this.FilteredUsersCollection.Add(user);
                    }
                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "InitializeFilteredUsersCollection");
            }
        }

        /// <summary>
        /// Initializes the Search Terms Collection
        /// </summary>
        private void InitializeNotificationStatusesCollection()
        {
            try
            {
                this.NotificationStatusesCollection = new ObservableCollection<string>();
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    this.NotificationStatusesCollection.Add("Yes");
                    this.NotificationStatusesCollection.Add("No");
                }));

            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "InitializeNotificationStatusesCollection");
            }
        }

        /// <summary>
        /// Initializes the Search Terms Collection
        /// </summary>
        private void InitializeUserStatusesCollection()
        {
            try
            {
                this.UserStatusesCollection = new ObservableCollection<string>();
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    this.UserStatusesCollection.Add("Active");
                    this.UserStatusesCollection.Add("Revoked");
                }));

            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "InitializeNotificationStatusesCollection");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshUI()
        {
            try
            {
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    AutoFXUsers = _helper.BuildUsersList();
                    AutoFXUsers = AutoFXUsers.OrderBy(x => x.ID).ToList();
                    AutoFXUsers.Reverse();

                    this.FilteredUsersCollection.Clear();

                    foreach (var user in AutoFXUsers)
                    {
                        this.FilteredUsersCollection.Add(user);
                    }
                    ResetTimer();
                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "RefreshUI");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SortBy(string sortBy)
        {
            try
            {
                List<User> sortedCollection = null;

                if (sortBy == "ID")
                {
                    sortedCollection = this.FilteredUsersCollection.OrderBy(x => x.ID).ToList();
                }
                else if (sortBy == "Account ID")
                {
                    sortedCollection = this.FilteredUsersCollection.OrderBy(x => x.AccountNumber).ToList();
                }
                else if (sortBy == "Key String")
                {
                    sortedCollection = this.FilteredUsersCollection.OrderBy(x => x.KeyString).ToList();
                }
                else if (sortBy == "Email")
                {
                    sortedCollection = this.FilteredUsersCollection.OrderBy(x => x.Email).ToList();
                }
                else if (sortBy == "Status")
                {
                    sortedCollection = this.FilteredUsersCollection.OrderBy(x => x.Status).ToList();
                }
                else
                {
                    sortedCollection = this.FilteredUsersCollection.ToList();
                }

                if (_sortingState % 2 != 0)
                {
                    sortedCollection.Reverse();
                    _sortingState++;
                }
                else
                {
                    _sortingState--;
                }

                this.FilteredUsersCollection.Clear();

                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                        {
                                                                                            foreach (var user in sortedCollection)
                                                                                            {
                                                                                                this.FilteredUsersCollection.Add(user);
                                                                                            }
                                                                                        }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SortBy");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RefreshUsersTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RefreshUI();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetTimer()
        {
            _refreshUsersTimer.Stop();
            _refreshUsersTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radioButton"></param>
        public void RadioButtonChecked(string radioButton)
        {
            if(radioButton == "All")
            {
                IsAllUsersChecked = true;
                IsActiveUsersChecked = false;
                IsRevokedUsersChecked = false;
            }
            else if (radioButton == "Active")
            {
                IsAllUsersChecked = false;
                IsActiveUsersChecked = true;
                IsRevokedUsersChecked = false;
            }
            else if (radioButton == "Revoked")
            {
                IsAllUsersChecked = false;
                IsActiveUsersChecked = false;
                IsRevokedUsersChecked = true;
            }
        }

        private void ResetUserInformation()
        {
            this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.ID = 0;
                this.Email = "";
                this.SelectedStatus = "";
                this.Account = "";
                this.Key = "";
                this.SelectedNotificationMode = "";
                this.AlternativeEmail = "";
            }));
        }
    }
}
