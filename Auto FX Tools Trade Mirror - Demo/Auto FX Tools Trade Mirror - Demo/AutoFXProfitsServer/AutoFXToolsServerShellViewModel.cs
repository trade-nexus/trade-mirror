using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AutoFXProfitsServer.Commands;
using Microsoft.Practices.Unity;
using TraceSourceLogger;
using Timer = System.Timers.Timer;

namespace AutoFXProfitsServer
{
    public class AutoFXToolsServerShellViewModel : DependencyObject
    {
        private static readonly Type OType = typeof(AutoFXToolsServerShellViewModel);

        #region Command Objects

        public ICommand SaveTemplateChangesCommand { get; set; }
        public ICommand SearchGoCommand { get; set; }
        public ICommand UndoEmailTemplateCommand { get; set; }
        public ICommand AddNewUserCommand { get; set; }
        public ICommand EditUserCommand { get; set; }
        public ICommand SaveUserCommand { get; set; }
        public ICommand SendManualEmailCommand { get; set; }
        public ICommand ExportUserCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SortByIDCommand { get; set; }
        public ICommand SortByAccountIDCommand { get; set; }
        public ICommand SortByKeyStringCommand { get; set; }
        public ICommand SortByEmailCommand { get; set; }
        public ICommand SortByStatusCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        #endregion

        private DBHelper _helper = null;
        //private TradeMirrorService _tradeMirrorService = null;

        public List<User> AutoFXUsers { get; set; }
        private static TradeMirrorService _service = null;
        private MailingHelper _mailingHelper;

        private bool _servcieStatus = false;
        private bool _editStatus = false;
        private UserWindow _userWindow;

        private int _sortingState = 0;

        public bool EditStatus
        {
            get { return this._editStatus; }
            set { this._editStatus = value; }
        }

        private static Timer _refreshUsersTimer;
        private const int RefreshPeriod = 150;

        /// <summary>
        /// Holds reference to UI dispatcher
        /// </summary>
        private readonly Dispatcher _currentDispatcher;

        private DeleteUserConfirmation _deleteUserConfirmationWindow;

        #region SearchItem

        public static readonly DependencyProperty SearchItemProperty =
            DependencyProperty.Register("SearchItem", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string SearchItem
        {
            get { return (string) GetValue(SearchItemProperty); }
            set { SetValue(SearchItemProperty, value); }
        }

        #endregion

        #region SearchTermsCollection

        public static readonly DependencyProperty SearchTermsCollectionProperty =
            DependencyProperty.Register("SearchTermsCollection", typeof (ObservableCollection<string>), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> SearchTermsCollection
        {
            get { return (ObservableCollection<string>) GetValue(SearchTermsCollectionProperty); }
            set { SetValue(SearchTermsCollectionProperty, value); }
        }

        #endregion

        #region NotificationStatusesCollection

        public static readonly DependencyProperty NotificationStatusesCollectionProperty =
            DependencyProperty.Register("NotificationStatusesCollection", typeof (ObservableCollection<string>), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> NotificationStatusesCollection
        {
            get { return (ObservableCollection<string>) GetValue(NotificationStatusesCollectionProperty); }
            set { SetValue(NotificationStatusesCollectionProperty, value); }
        }

        #endregion

        #region UserStatusesCollection

        public static readonly DependencyProperty UserStatusesCollectionProperty =
            DependencyProperty.Register("UserStatusesCollection", typeof (ObservableCollection<string>), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> UserStatusesCollection
        {
            get { return (ObservableCollection<string>) GetValue(UserStatusesCollectionProperty); }
            set { SetValue(UserStatusesCollectionProperty, value); }
        }

        #endregion

        #region SelectedSearchType

        public static readonly DependencyProperty SelectedSearchTypeProperty =
            DependencyProperty.Register("SelectedSearchType", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string SelectedSearchType
        {
            get { return (string) GetValue(SelectedSearchTypeProperty); }
            set { SetValue(SelectedSearchTypeProperty, value); }
        }

        #endregion

        #region FilteredUsersCollection

        public static readonly DependencyProperty FilteredUsersCollectionProperty =
            DependencyProperty.Register("FilteredUsersCollection", typeof (ObservableCollection<User>), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata((new ObservableCollection<User>())));

        public ObservableCollection<User> FilteredUsersCollection
        {
            get { return (ObservableCollection<User>) GetValue(FilteredUsersCollectionProperty); }
            set { SetValue(FilteredUsersCollectionProperty, value); }
        }

        #endregion

        #region EmailTemplateNames

        public static readonly DependencyProperty EmailTemplateNamesCollectionProperty =
            DependencyProperty.Register("EmailTemplateNamesCollection", typeof(ObservableCollection<string>), typeof(AutoFXToolsServerShellViewModel), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> EmailTemplateNamesCollection
        {
            get { return (ObservableCollection<string>)GetValue(EmailTemplateNamesCollectionProperty); }
            set { SetValue(EmailTemplateNamesCollectionProperty, value); }
        }

        #endregion

        #region SelectedEmailTemplateName

        public static readonly DependencyProperty SelectedEmailTemplateNameProperty =
            DependencyProperty.Register("SelectedEmailTemplateName", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string SelectedEmailTemplateName
        {
            get { return (string) GetValue(SelectedEmailTemplateNameProperty); }
            set { SetValue(SelectedEmailTemplateNameProperty, value); }
        }
        
        #endregion

        #region SelectedEmailTemplate

        public static readonly DependencyProperty SelectedEmailTemplateProperty =
            DependencyProperty.Register("SelectedEmailTemplate", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string SelectedEmailTemplate
        {
            get { return (string) GetValue(SelectedEmailTemplateProperty); }
            set { SetValue(SelectedEmailTemplateProperty, value); }
        }

        #endregion

        #region EmailTemplateViewEnabled

        public static readonly DependencyProperty EmailTemplateViewEnabledProperty =
            DependencyProperty.Register("EmailTemplateViewEnabled", typeof (bool), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(bool)));

        public bool EmailTemplateViewEnabled
        {
            get { return (bool) GetValue(EmailTemplateViewEnabledProperty); }
            set { SetValue(EmailTemplateViewEnabledProperty, value); }
        }

        #endregion

        #region UsersCollection

        public ObservableCollection<User> UsersCollection { get; set; }

        #endregion

        #region TotalClients

        public static readonly DependencyProperty TotalClientsProperty =
            DependencyProperty.Register("TotalClients", typeof (int), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(int)));

        public int TotalClients
        {
            get { return (int) GetValue(TotalClientsProperty); }
            set { SetValue(TotalClientsProperty, value); }
        }

        #endregion

        #region Service

        public static readonly DependencyProperty ServiceProperty =
            DependencyProperty.Register("Service", typeof (TradeMirrorService), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(TradeMirrorService)));

        public TradeMirrorService Service
        {
            get { return (TradeMirrorService) GetValue(ServiceProperty); }
            set { SetValue(ServiceProperty, value); }
        }

        #endregion

        #region ConnectedClients

        public static readonly DependencyProperty ConnectedClientsProperty =
            DependencyProperty.Register("ConnectedClients", typeof (int), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(int)));

        public int ConnectedClients
        {
            get { return (int) GetValue(ConnectedClientsProperty); }
            set { SetValue(ConnectedClientsProperty, value); }
        }

        #endregion

        #region ActiveClients

        public static readonly DependencyProperty ActiveClientsProperty =
            DependencyProperty.Register("ActiveClients", typeof (int), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(int)));

        public int ActiveClients
        {
            get { return (int) GetValue(ActiveClientsProperty); }
            set { SetValue(ActiveClientsProperty, value); }
        }

        #endregion

        #region RevokedClients

        public static readonly DependencyProperty RevokedClientsProperty =
            DependencyProperty.Register("RevokedClients", typeof (int), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(int)));

        public int RevokedClients
        {
            get { return (int) GetValue(RevokedClientsProperty); }
            set { SetValue(RevokedClientsProperty, value); }
        }

        #endregion

        #region IsAllUsersChecked

        public static readonly DependencyProperty IsAllUsersCheckedProperty =
            DependencyProperty.Register("IsAllUsersChecked", typeof (bool), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(true));

        public bool IsAllUsersChecked
        {
            get { return (bool) GetValue(IsAllUsersCheckedProperty); }
            set { SetValue(IsAllUsersCheckedProperty, value); }
        }

        #endregion

        #region IsActiveUsersChecked

        public static readonly DependencyProperty IsActiveUsersCheckedProperty =
            DependencyProperty.Register("IsActiveUsersChecked", typeof (bool), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(bool)));

        public bool IsActiveUsersChecked
        {
            get { return (bool) GetValue(IsActiveUsersCheckedProperty); }
            set { SetValue(IsActiveUsersCheckedProperty, value); }
        }

        #endregion

        #region IsRevokedUsersChecked

        public static readonly DependencyProperty IsRevokedUsersCheckedProperty =
            DependencyProperty.Register("IsRevokedUsersChecked", typeof (bool), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(bool)));

        public bool IsRevokedUsersChecked
        {
            get { return (bool) GetValue(IsRevokedUsersCheckedProperty); }
            set { SetValue(IsRevokedUsersCheckedProperty, value); }
        }

        #endregion

        #region SelectedUser

        public static readonly DependencyProperty SelectedUserProperty =
            DependencyProperty.Register("SelectedUser", typeof (object), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(object)));

        public object SelectedUser
        {
            get { return (object) GetValue(SelectedUserProperty); }
            set { SetValue(SelectedUserProperty, value); }
        }

        #endregion

        #region ID

        public int ID { get; set; }

        #endregion

        #region Email

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string Email
        {
            get { return (string) GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        #endregion

        #region Account

        public static readonly DependencyProperty AccountProperty =
            DependencyProperty.Register("Account", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string Account
        {
            get { return (string) GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion

        #region Key

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string Key
        {
            get { return (string) GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        #endregion

        #region SelectedStatus

        public static readonly DependencyProperty SelectedStatusProperty =
            DependencyProperty.Register("SelectedStatus", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string SelectedStatus
        {
            get { return (string) GetValue(SelectedStatusProperty); }
            set { SetValue(SelectedStatusProperty, value); }
        }

        #endregion

        #region AlternateEmail

        public static readonly DependencyProperty AlternativeEmailProperty =
            DependencyProperty.Register("AlternativeEmail", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(""));

        public string AlternativeEmail
        {
            get { return (string) GetValue(AlternativeEmailProperty); }
            set { SetValue(AlternativeEmailProperty, value); }
        }

        #endregion

        #region SelectedNotificationMode

        public static readonly DependencyProperty SelectedNotificationModeProperty =
            DependencyProperty.Register("SelectedNotificationMode", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string SelectedNotificationMode
        {
            get { return (string) GetValue(SelectedNotificationModeProperty); }
            set { SetValue(SelectedNotificationModeProperty, value); }
        }

        #endregion

        #region ConnectedUsers

        public static readonly DependencyProperty ConnectedUsersProperty =
            DependencyProperty.Register("ConnectedUsers", typeof (ObservableCollection<User>), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(ObservableCollection<User>)));

        public ObservableCollection<User> ConnectedUsers
        {
            get { return (ObservableCollection<User>) GetValue(ConnectedUsersProperty); }
            set { SetValue(ConnectedUsersProperty, value); }
        }

        #endregion

        #region ManualEmailSubject

        public static readonly DependencyProperty ManualEmailSubjectProperty =
            DependencyProperty.Register("ManualEmailSubject", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string ManualEmailSubject
        {
            get { return (string) GetValue(ManualEmailSubjectProperty); }
            set { SetValue(ManualEmailSubjectProperty, value); }
        }

        #endregion

        #region EnteredManualEmail

        public static readonly DependencyProperty EnteredManualEmailProperty =
            DependencyProperty.Register("EnteredManualEmail", typeof (string), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(string)));

        public string EnteredManualEmail
        {
            get { return (string) GetValue(EnteredManualEmailProperty); }
            set { SetValue(EnteredManualEmailProperty, value); }
        }

        #endregion

        #region ConnectedUsersCount

        public static readonly DependencyProperty ConnectedUsersCountProperty =
            DependencyProperty.Register("ConnectedUsersCount", typeof (int), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(int)));

        public int ConnectedUsersCount
        {
            get { return (int) GetValue(ConnectedUsersCountProperty); }
            set { SetValue(ConnectedUsersCountProperty, value); }
        }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AutoFXToolsServerShellViewModel()
        {
            #region Command Initializations

            this.SaveTemplateChangesCommand = new SaveTemplateChangesCommand(this);
            this.SearchGoCommand = new SearchGoCommand(this);
            this.UndoEmailTemplateCommand = new UndoEmailTemplateCommand(this);
            this.AddNewUserCommand = new AddNewUserCommand(this);
            this.EditUserCommand=new EditUserCommand(this);
            this.SaveUserCommand=new SaveUserCommand(this);
            this.SendManualEmailCommand = new SendManualEmailCommand(this);
            this.ExportUserCommand = new ExportUserCommand(this);
            this.RefreshCommand = new RefreshCommand(this);
            this.SortByIDCommand = new SortByIDCommand(this);
            this.SortByAccountIDCommand = new SortByAccountIDCommand(this);
            this.SortByKeyStringCommand = new SortByKeyStringCommand(this);
            this.SortByEmailCommand = new SortByEmailCommand(this);
            this.SortByStatusCommand = new SortByStatusCommand(this);
            this.DeleteCommand = new DeleteCommand(this);

            #endregion

            SearchHelper.ClientSubscribed += ClientSubscribed;
            SearchHelper.ClientUnSubscribed += ClientUnSubscribed;
            ConnectedUsersCount = 0;

            _refreshUsersTimer = new Timer(RefreshPeriod * 1000);
            _refreshUsersTimer.Elapsed += RefreshUsersTimerElapsed;
            _refreshUsersTimer.AutoReset = true;
            _refreshUsersTimer.Enabled = true;

            ConnectedUsers = new ObservableCollection<User>();

            this._currentDispatcher = Dispatcher.CurrentDispatcher;

            var connectionManager = new ConnectionManager();
            _helper = new DBHelper(connectionManager);
            AutoFXUsers = _helper.BuildUsersList();

            _mailingHelper = new MailingHelper(AutoFXUsers);

            Service = new TradeMirrorService();
            ThreadPool.QueueUserWorkItem(InitializeService, Service);

            InitializeSearchTermsCollection();
            InitializeEmailTemplateNamesCollection();
            InitializeFilteredUsersCollection();
            InitializeNotificationStatusesCollection();
            InitializeUserStatusesCollection();

            SetActiveUsersOnUI();
            SetRevokedUsersOnUI();
        }

        private void ClientUnSubscribed(User user)
        {
            try
            {
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                        {
                                                                                            if (ConnectedUsers.Contains(user))
                                                                                            {
                                                                                                ConnectedUsers.Remove(user);
                                                                                                ConnectedUsersCount--;
                                                                                            }
                                                                                        }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ClientUnSubscribed");
            }
        }

        private void ClientSubscribed(User user)
        {
            try
            {
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                        {
                                                                                            if (!ConnectedUsers.Contains(user))
                                                                                            {
                                                                                                ConnectedUsers.Add(user);
                                                                                                ConnectedUsersCount++;
                                                                                            }
                                                                                        }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ClientSubscribed");
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
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
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
        private void InitializeEmailTemplateNamesCollection()
        {
            try
            {
                this.EmailTemplateNamesCollection = new ObservableCollection<string>();
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                        {
                                                                                            this.EmailTemplateNamesCollection.Add("New");
                                                                                            this.EmailTemplateNamesCollection.Add("Modify");
                                                                                            this.EmailTemplateNamesCollection.Add("Partial Close");
                                                                                            this.EmailTemplateNamesCollection.Add("Exit");
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
                this.FilteredUsersCollection=new ObservableCollection<User>();
                AutoFXUsers = AutoFXUsers.OrderBy(x => x.ID).ToList();
                AutoFXUsers.Reverse();

                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                        {
                                                                                            foreach (var user in AutoFXUsers)
                                                                                            {
                                                                                                this.FilteredUsersCollection.Add(user);
                                                                                            }
                                                                                            this.TotalClients = this.FilteredUsersCollection.Count;
                                                                                        }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "InitializeFilteredUsersCollection");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EmailTemplateSelectionChanged()
        {
            try
            {
                EmailTemplateViewEnabled = false;

                string command = string.Empty;
                if (SelectedEmailTemplateName == "New")
                {
                    command = "OP";
                }
                else if (SelectedEmailTemplateName == "Modify")
                {
                    command = "MO";
                }
                else if (SelectedEmailTemplateName == "Partial Close")
                {
                    command = "PL";
                }
                else if (SelectedEmailTemplateName == "Exit")
                {
                    command = "CL";
                }

                string emailTemplate = _mailingHelper.GetEmailTemplate(command);
                Logger.Info("Email Template Selected = " + emailTemplate, OType.FullName, "EmailTemplateSelectionChanged");
                SelectedEmailTemplate = emailTemplate;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "EmailTemplateSelectionChanged");
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void SelectionFilterSelectionChanged()
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
                else if(IsActiveUsersChecked)
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
        /// 
        /// </summary>
        private void SetActiveUsersOnUI()
        {
            try
            {
                int numberOfActiveUsers = SearchHelper.GetActiveUsers(AutoFXUsers).Count;

                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                                                                                       {
                                                                                           this.ActiveClients = numberOfActiveUsers;
                                                                                       }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SetActiveUsersOnUI");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetRevokedUsersOnUI()
        {
            try
            {
                int numberOfRevokedUsers = SearchHelper.GetRevokedUsers(AutoFXUsers).Count;

                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    this.RevokedClients = numberOfRevokedUsers;
                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SetRevokedUsersOnUI");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradeMirrorServiceObject"></param>
        private void InitializeService(Object tradeMirrorServiceObject)
        {
            try
            {
                var tradeMirrorService = (TradeMirrorService) tradeMirrorServiceObject;
                tradeMirrorService.Start();
                _servcieStatus = true;
                while (_servcieStatus)
                {
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "InitializeService");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradeMirrorServiceObject"></param>
        private void StopService(Object tradeMirrorServiceObject)
        {
            try
            {
                var tradeMirrorService = (TradeMirrorService)tradeMirrorServiceObject;
                tradeMirrorService.Stop();
                _servcieStatus = false;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "StopService");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void FreeResources()
        {
            //StopService(_tradeMirrorService);
            StopService(Service);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveTemplateChanges()
        {
            _mailingHelper.SaveTemplate(SelectedEmailTemplateName, SelectedEmailTemplate);
            EmailTemplateViewEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void EditTemplate()
        {
            EmailTemplateViewEnabled = true;
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
                if(String.IsNullOrEmpty(AlternativeEmail))
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

                if(selectedUser != null)
                {
                    this.ID = selectedUser.ID;
                    this.Email = selectedUser.Email;
                    this.SelectedStatus = selectedUser.Status;
                    this.Account = selectedUser.AccountNumber;
                    this.Key = selectedUser.KeyString;
                    this.SelectedNotificationMode = selectedUser.SendNotifications;
                    this.AlternativeEmail = selectedUser.AlternativeEmail;
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
                    this.TotalClients = this.FilteredUsersCollection.Count;
                    _mailingHelper.UsersList = AutoFXUsers;
                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "UpdateUserList");
            }
        }

        public void SendManualEmail()
        {
            try
            {
                MailingHelper mailingHelper = new MailingHelper(AutoFXUsers);
                string receipientType = string.Empty;

                if (IsAllUsersChecked)
                {
                    receipientType = "All";
                }
                else if (IsActiveUsersChecked)
                {
                    receipientType = "Active";
                }
                else if (IsRevokedUsersChecked)
                {
                    receipientType = "Revoked";
                }

                if(mailingHelper.SendEmail(ManualEmailSubject, receipientType, EnteredManualEmail))
                {
                    MessageBox.Show("Manual Email Sent");
                }
                else
                {
                    MessageBox.Show("Manual Email Failed");
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SendManualEmail");
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

                if(!Directory.Exists(path))
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
                    if(autoFXUser.AlternativeEmail != autoFXUser.Email)
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
        public void RefreshUI()
        {
            try
            {
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    UpdateUserList();
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

                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
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
            Logger.Info("Refreshing Application Data", OType.FullName, "RefreshUsersTimerElapsed");
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
