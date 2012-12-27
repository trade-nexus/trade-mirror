using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AutoFXProfitsServer.Commands;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public class AutoFXToolsServerShellViewModel : DependencyObject
    {
        private static readonly Type OType = typeof(AutoFXToolsServerShellViewModel);

        public ICommand SaveTemplateChangesCommand { get; set; }
        public ICommand SearchGoCommand { get; set; }
        public ICommand UndoEmailTemplateCommand { get; set; }
        public ICommand AddNewUserCommand { get; set; }

        private DBHelper _helper = null;

        private static List<User> AutoFXUsers { get; set; }

        /// <summary>
        /// Holds reference to UI dispatcher
        /// </summary>
        private readonly Dispatcher _currentDispatcher;

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

        #region ConnectedClients

        public static readonly DependencyProperty ConnectedClientsProperty =
            DependencyProperty.Register("ConnectedClients", typeof (int), typeof (AutoFXToolsServerShellViewModel), new PropertyMetadata(default(int)));

        public int ConnectedClients
        {
            get { return (int) GetValue(ConnectedClientsProperty); }
            set { SetValue(ConnectedClientsProperty, value); }
        }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AutoFXToolsServerShellViewModel()
        {
            this.SaveTemplateChangesCommand = new SaveTemplateChangesCommand(this);
            this.SearchGoCommand = new SearchGoCommand(this);
            this.UndoEmailTemplateCommand = new UndoEmailTemplateCommand(this);
            this.AddNewUserCommand = new AddNewUserCommand(this);

            this._currentDispatcher = Dispatcher.CurrentDispatcher;

            var connectionManager = new ConnectionManager();
            _helper = new DBHelper(connectionManager);
            //User user = new User(1, 12345, "hello");
            //FilteredUsersCollection.Add(user);

            //TotalClients = 1000;
            ConnectedClients = 1;

            InitializeSearchTermsCollection();
            InitializeEmailTemplateNamesCollection();
            InitializeFilteredUsersCollection();
            
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
                AutoFXUsers = _helper.BuildUsersList();

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
            
        }
    }
}
