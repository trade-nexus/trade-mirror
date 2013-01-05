using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.ServiceModel;
using System.Windows.Media;
using System.Windows.Threading;
using TraceSourceLogger;

namespace AutoFXProfitsClientTerminal
{
    public class MainWindowViewModel : DependencyObject, ITradeMirrorCallback
    {
        private static readonly Type OType = typeof(MainWindowViewModel);

        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }

        private readonly InstanceContext _site;
        private TradeMirrorClient _client;

        private static Timer _heartbeatTimer;
        private const int AcceptedDelaySeconds = 60;

        /// <summary>
        /// Holds reference to UI dispatcher
        /// </summary>
        private readonly Dispatcher _currentDispatcher;

        #region AccountID

        public static readonly DependencyProperty AccountIDProperty =
            DependencyProperty.Register("AccountID", typeof (string), typeof (MainWindowViewModel), new PropertyMetadata(default(string)));

        public string AccountID
        {
            get { return (string) GetValue(AccountIDProperty); }
            set { SetValue(AccountIDProperty, value); }
        }

        #endregion

        #region KeyString
        
        public static readonly DependencyProperty KeyStringProperty =
            DependencyProperty.Register("KeyString", typeof (string), typeof (MainWindowViewModel), new PropertyMetadata(default(string)));

        public string KeyString
        {
            get { return (string) GetValue(KeyStringProperty); }
            set { SetValue(KeyStringProperty, value); }
        }

        #endregion

        #region Status

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof (string), typeof (MainWindowViewModel), new PropertyMetadata(default(string)));

        public string Status
        {
            get { return (string) GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        #endregion

        #region Color

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(MainWindowViewModel), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        #endregion

        #region IsSavePasswordChecked

        public static readonly DependencyProperty IsSavePasswordCheckedProperty =
            DependencyProperty.Register("IsSavePasswordChecked", typeof (bool), typeof (MainWindowViewModel), new PropertyMetadata(default(bool)));

        public bool IsSavePasswordChecked
        {
            get { return (bool) GetValue(IsSavePasswordCheckedProperty); }
            set { SetValue(IsSavePasswordCheckedProperty, value); }
        }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            try
            {
                this.ConnectCommand = new ConnectCommand(this);
                this.DisconnectCommand = new DisconnectCommand(this);

                this._currentDispatcher = Dispatcher.CurrentDispatcher;

                // Create a client
                _site = new InstanceContext(null, this);
                _client = new TradeMirrorClient(_site);

                _heartbeatTimer = new Timer(AcceptedDelaySeconds * 1000);
                _heartbeatTimer.Elapsed += HeartbeatTimerElapsed;
                _heartbeatTimer.AutoReset = true;
                _heartbeatTimer.Enabled = true;

                UpdateUI("Disconnected");
                if (!InitializeCredentials())
                {
                    SetAccountID();
                }
                else
                {
                    IsSavePasswordChecked = true;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "MainWindowViewModel");
            }
        }

        /// <summary>
        /// Connect the client to the server
        /// </summary>
        public void ConnectToServer()
        {
            try
            {
                if (_client.Subscribe(AccountID, KeyString, Convert.ToInt32(AccountID)))
                {
                    Logger.Info("Subscribed", OType.FullName, "ConnectToServer");

                    UpdateUI("Connected");

                    if (IsSavePasswordChecked)
                    {
                        SaveAccountCredentials();
                    }
                    else
                    {
                        DeleteAccountCredentials();
                    }
                }
                else
                {
                    Logger.Info("Invalid Credentials", OType.FullName, "ConnectToServer");
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ConnectToServer");
            }
        }

        /// <summary>
        /// Disconnect the client from the server
        /// </summary>
        public void DisconnectFromServer()
        {
            try
            {
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                        {
                                                                                            if (
                                                                                                _client.Unsubscribe(AccountID, KeyString,Convert.ToInt32(AccountID)))
                                                                                            {
                                                                                                Logger.Info("Unsubscribed",OType.FullName,"DisconnectFromServer");

                                                                                                UpdateUI("Disconnected");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Logger.Info("Invalid Credentials",OType.FullName,"DisconnectFromServer");
                                                                                            }
                                                                                        }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "DisconnectFromServer");
            }
        }

        /// <summary>
        /// Method called when a new signal arrives
        /// </summary>
        /// <param name="signalInformation"></param>
        public void NewSignal(string signalInformation)
        {
            if (signalInformation.Contains("___autofxtools trademirror___Alive___"))
            {
                Logger.Debug("Heartbeat. = " + signalInformation, OType.FullName, "PublishNewSignal");
                return;
            }

            Logger.Debug("New Signal Received = " + signalInformation, OType.FullName, "PublishNewSignal");
            PlaceOrder(signalInformation);
        }

        /// <summary>
        /// Updates output on UI
        /// </summary>
        /// <param name="newStatus"></param>
        private void UpdateUI(string newStatus)
        {
            try
            {
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                                                                                       {
                                                                                           this.Status = newStatus;

                                                                                           if (Status == "Connected")
                                                                                           {
                                                                                               Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 0));
                                                                                           }
                                                                                           else if (Status == "Disconnected")
                                                                                           {
                                                                                               Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
                                                                                           }

                                                                                       }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "UpdateUI");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetAccountID()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\accountinfo.txt";

                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    StreamReader streamReader = new StreamReader(fs);

                    string tempString = string.Empty;

                    while ((tempString = streamReader.ReadLine()) != null)
                    {
                        if (tempString.Contains("accountNumber"))
                        {
                            string[] tempArray = tempString.Split(':');
                            this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                                    {
                                                                                                        AccountID =
                                                                                                            tempArray[1]
                                                                                                                .
                                                                                                                Trim();

                                                                                                    }));
                        }
                        break;
                    }

                    streamReader.Close();
                    fs.Close();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SetAccountID");
            }            
        }

        /// <summary>
        /// Saves order into a text file
        /// </summary>
        public void PlaceOrder(string orderInfo)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\orders.csv";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (File.Create(path))
                {
                }

                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None);
                var streamWriter = new StreamWriter(fileStream);

                streamWriter.Write(orderInfo);
                Logger.Debug("Signal Information Written to file", OType.FullName, "PlaceOrder");

                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "PlaceOrder");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveAccountCredentials()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\trademirrorcredentials.txt";

                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                    StreamWriter streamWriter = new StreamWriter(fs);

                    streamWriter.Write(AccountID + " : " + KeyString);
                    Logger.Debug("User Credentials saved. Account ID = " + AccountID + " | Keystring = " + KeyString, OType.FullName, "PlaceOrder");

                    streamWriter.Close();
                    fs.Close();
                }   
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SaveAccountCredentials");
            }   
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteAccountCredentials()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\trademirrorcredentials.txt";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                Logger.Debug("User Credentials Deleted" + KeyString, OType.FullName, "SaveAccountCredentials");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SaveAccountCredentials");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool InitializeCredentials()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\\trademirrorcredentials.txt";

            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader streamReader = new StreamReader(fs);

                string tempString = streamReader.ReadLine();

                string[] tempArray = tempString.Split(':');

                AccountID = tempArray[0].Trim();
                KeyString = tempArray[1].Trim();

                Logger.Debug("User Credentials Initialized. Account ID = " + AccountID + " | Keystring = " + KeyString, OType.FullName, "InitializeCredentials");

                streamReader.Close();
                fs.Close();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HeartbeatTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.DisconnectFromServer();
            Logger.Debug("Connection lost to server", OType.FullName, "HeartbeatTimerElapsed");
        }

        /// <summary>
        /// 
        /// </summary>
        public void FreeResources()
        {
            this.DisconnectFromServer();
        }
    }
}
