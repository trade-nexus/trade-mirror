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
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TraceSourceLogger;
using UpDownSingnalsClientTerminal.Command;
using UpDownSingnalsClientTerminal.Views;

namespace UpDownSingnalsClientTerminal.ViewModels
{
    public class ApplicationViewModel : DependencyObject, IDisposable, IUpDownSignalsCallback
    {
        private static readonly Type OType = typeof(ApplicationViewModel);

        private const string HeartBeatMessage = "___up down signals trade copier___Alive___";

        private readonly InstanceContext _site;
        private UpDownSignalsClient _client;

        private ClientTerminal _clientTerminal;

        #region Username

        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof (string), typeof (ApplicationViewModel), new PropertyMetadata(default(string)));

        public string Username
        {
            get { return (string) GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        #endregion
        
        #region Status

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(ApplicationViewModel), new PropertyMetadata(default(string)));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        #endregion

        #region Color

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(ApplicationViewModel), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        #endregion

        #region IsSavePasswordChecked

        public static readonly DependencyProperty IsSavePasswordCheckedProperty =
            DependencyProperty.Register("IsSavePasswordChecked", typeof(bool), typeof(ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool IsSavePasswordChecked
        {
            get { return (bool)GetValue(IsSavePasswordCheckedProperty); }
            set { SetValue(IsSavePasswordCheckedProperty, value); }
        }

        #endregion

        #region IsSystemEnabledChecked

        public static readonly DependencyProperty IsSystemEnabledCheckedProperty =
            DependencyProperty.Register("IsSystemEnabledChecked", typeof (bool), typeof (ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool IsSystemEnabledChecked
        {
            get { return (bool) GetValue(IsSystemEnabledCheckedProperty); }
            set { SetValue(IsSystemEnabledCheckedProperty, value); }
        }

        #endregion

        #region IsAfterHoursChecked

        public static readonly DependencyProperty IsAfterHoursCheckedProperty =
            DependencyProperty.Register("IsAfterHoursChecked", typeof (bool), typeof (ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool IsAfterHoursChecked
        {
            get { return (bool) GetValue(IsAfterHoursCheckedProperty); }
            set { SetValue(IsAfterHoursCheckedProperty, value); }
        }

        #endregion

        #region IsMnaualExitChecked

        public static readonly DependencyProperty IsMnaualExitCheckedProperty =
            DependencyProperty.Register("IsMnaualExitChecked", typeof (bool), typeof (ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool IsMnaualExitChecked
        {
            get { return (bool) GetValue(IsMnaualExitCheckedProperty); }
            set { SetValue(IsMnaualExitCheckedProperty, value); }
        }

        #endregion

        #region ExitAfterHours

        public static readonly DependencyProperty ExitAfterHoursProperty =
            DependencyProperty.Register("ExitAfterHours", typeof (string), typeof (ApplicationViewModel), new PropertyMetadata(default(string)));

        public string ExitAfterHours
        {
            get { return (string) GetValue(ExitAfterHoursProperty); }
            set { SetValue(ExitAfterHoursProperty, value); }
        }

        #endregion

        #region IsAfterStopsChecked

        public static readonly DependencyProperty IsAfterStopsCheckedProperty =
            DependencyProperty.Register("IsAfterStopsChecked", typeof (bool), typeof (ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool IsAfterStopsChecked
        {
            get { return (bool) GetValue(IsAfterStopsCheckedProperty); }
            set { SetValue(IsAfterStopsCheckedProperty, value); }
        }

        #endregion

        #region IsStopLossChecked

        public static readonly DependencyProperty IsStopLossCheckedProperty =
            DependencyProperty.Register("IsStopLossChecked", typeof (bool), typeof (ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool IsStopLossChecked
        {
            get { return (bool) GetValue(IsStopLossCheckedProperty); }
            set { SetValue(IsStopLossCheckedProperty, value); }
        }

        #endregion

        #region StopLossBalance

        public static readonly DependencyProperty StopLossBalanceProperty =
            DependencyProperty.Register("StopLossBalance", typeof (string), typeof (ApplicationViewModel), new PropertyMetadata(default(string)));

        public string StopLossBalance
        {
            get { return (string) GetValue(StopLossBalanceProperty); }
            set { SetValue(StopLossBalanceProperty, value); }
        }

        #endregion

        #region IsTakeProfitChecked

        public static readonly DependencyProperty IsTakeProfitCheckedProperty =
            DependencyProperty.Register("IsTakeProfitChecked", typeof (bool), typeof (ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool IsTakeProfitChecked
        {
            get { return (bool) GetValue(IsTakeProfitCheckedProperty); }
            set { SetValue(IsTakeProfitCheckedProperty, value); }
        }

        #endregion

        #region TakeProfitBalance

        public static readonly DependencyProperty TakeProfitBalanceProperty =
            DependencyProperty.Register("TakeProfitBalance", typeof (string), typeof (ApplicationViewModel), new PropertyMetadata(default(string)));

        public string TakeProfitBalance
        {
            get { return (string) GetValue(TakeProfitBalanceProperty); }
            set { SetValue(TakeProfitBalanceProperty, value); }
        }

        #endregion

        #region IsMnaualStopsChecked

        public static readonly DependencyProperty IsMnaualStopsCheckedProperty =
            DependencyProperty.Register("IsMnaualStopsChecked", typeof (bool), typeof (ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool IsMnaualStopsChecked
        {
            get { return (bool) GetValue(IsMnaualStopsCheckedProperty); }
            set { SetValue(IsMnaualStopsCheckedProperty, value); }
        }

        #endregion

        #region Command

        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// IDisposable.Dispose implementation, calls Dispose(true).
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose worker method. Handles graceful shutdown of the
        /// client even if it is an faulted state.
        /// </summary>
        /// <param name="disposing">Are we disposing (alternative
        /// is to be finalizing)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (_client.State != CommunicationState.Faulted)
                    {
                        _client.Close();
                    }
                }
                finally
                {
                    if (_client.State != CommunicationState.Closed)
                    {
                        _client.Abort();
                    }
                }
            }
        }

        #endregion

        #region TotalBalanceAllocation

        public static readonly DependencyProperty TotalBalanceAllocationProperty =
            DependencyProperty.Register("TotalBalanceAllocation", typeof (string), typeof (ApplicationViewModel), new PropertyMetadata(default(string)));

        public string TotalBalanceAllocation
        {
            get { return (string) GetValue(TotalBalanceAllocationProperty); }
            set { SetValue(TotalBalanceAllocationProperty, value); }
        }

        #endregion

        #region TradeSizeTypeCollection

        public static readonly DependencyProperty TradeSizeTypeCollectionProperty =
            DependencyProperty.Register("TradeSizeTypeCollection", typeof (List<string>), typeof (ApplicationViewModel), new PropertyMetadata(default(List<string>)));

        public List<string> TradeSizeTypeCollection
        {
            get { return (List<string>) GetValue(TradeSizeTypeCollectionProperty); }
            set { SetValue(TradeSizeTypeCollectionProperty, value); }
        }

        #endregion

        #region SelectedTradeSizeType

        public static readonly DependencyProperty SelectedTradeSizeTypeProperty =
            DependencyProperty.Register("SelectedTradeSizeType", typeof (string), typeof (ApplicationViewModel), new PropertyMetadata(default(string)));

        public string SelectedTradeSizeType
        {
            get { return (string) GetValue(SelectedTradeSizeTypeProperty); }
            set { SetValue(SelectedTradeSizeTypeProperty, value); }
        }

        #endregion

        #region TradeSizeType

        public static readonly DependencyProperty TradeSizeTypeProperty =
            DependencyProperty.Register("TradeSizeType", typeof (string), typeof (ApplicationViewModel), new PropertyMetadata(default(string)));

        public string TradeSizeType
        {
            get { return (string) GetValue(TradeSizeTypeProperty); }
            set { SetValue(TradeSizeTypeProperty, value); }
        }

        #endregion

        #region TradeSizeValue

        public static readonly DependencyProperty TradeSizeValueProperty =
            DependencyProperty.Register("TradeSizeValue", typeof (string), typeof (ApplicationViewModel), new PropertyMetadata(default(string)));

        public string TradeSizeValue
        {
            get { return (string) GetValue(TradeSizeValueProperty); }
            set { SetValue(TradeSizeValueProperty, value); }
        }

        #endregion

        #region TradeSizeValueVisibility

        public static readonly DependencyProperty TradeSizeValueVisibilityProperty =
            DependencyProperty.Register("TradeSizeValueVisibility", typeof (bool), typeof (ApplicationViewModel), new PropertyMetadata(default(bool)));

        public bool TradeSizeValueVisibility
        {
            get { return (bool) GetValue(TradeSizeValueVisibilityProperty); }
            set { SetValue(TradeSizeValueVisibilityProperty, value); }
        }

        #endregion

        /// <summary>
        /// Holds reference to UI dispatcher
        /// </summary>
        private readonly Dispatcher _currentDispatcher;

        private static Timer _heartbeatTimer;
        private const int AcceptedDelaySeconds = 60;

        
        public ApplicationViewModel()
        {
            try
            {
                this.ConnectCommand = new ConnectCommand(this);
                this.DisconnectCommand = new DisconnectCommand(this);

                this._currentDispatcher = Dispatcher.CurrentDispatcher;
                _clientTerminal = new ClientTerminal(this);

                TradeSizeTypeCollection = new List<string>();

                // Create a client
                _site = new InstanceContext(null, this);
                _site.Closed += CommunicationObjectOnClosed;
                _site.Faulted += CommunicationObjectOnClosed;

                _client = new UpDownSignalsClient(_site);

                _heartbeatTimer = new Timer(AcceptedDelaySeconds * 1000);
                _heartbeatTimer.Elapsed += HeartbeatTimerElapsed;
                _heartbeatTimer.AutoReset = true;

                UpdateUI("Disconnected");
                
                InitializeClientTerminalUI();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ApplicationViewModel");
            }
        }

        #region User Interface Method Group

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
        
        #endregion

        #region WCF Service Method Group

        /// <summary>
        /// Connect the client to the server
        /// </summary>
        public void ConnectToServer()
        {
            try
            {
                string suffixes = _client.Subscribe(Username);

                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    if (suffixes != "FAILED")
                    {
                        Logger.Info("Subscribed", OType.FullName, "ConnectToServer");

                        UpdateUI("Connected");

                        SetSuffixes(suffixes);

                        _heartbeatTimer.Enabled = true;

                        SpawnSettingWindow();
                    }
                    else
                    {
                        Logger.Info("Invalid Credentials", OType.FullName, "ConnectToServer");
                    }
                }));
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
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    if (_client.Unsubscribe(Username))
                    {
                        Logger.Info("Unsubscribed", OType.FullName, "DisconnectFromServer");

                        _heartbeatTimer.Enabled = false;
                        UpdateUI("Disconnected");

                        CloseSettingsWindow();
                    }
                    else
                    {
                        Logger.Info("Invalid Credentials", OType.FullName, "DisconnectFromServer");
                    }
                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "DisconnectFromServer");
                CloseSettingsWindow();
            }
        }

        /// <summary>
        /// Method called when a new signal arrives
        /// </summary>
        /// <param name="signalInformation"></param>
        public void NewSignal(string signalInformation)
        {
            try
            {
                if (signalInformation.Contains(HeartBeatMessage))
                {
                    Logger.Debug("Heartbeat. = " + signalInformation, OType.FullName, "PublishNewSignal");

                    this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action)(ResetTimer));
                    return;
                }

                Logger.Debug("New Signal Received = " + signalInformation, OType.FullName, "PublishNewSignal");

                string message = String.Empty;

                if(SplitSignal(signalInformation) != null)
                {
                    string[] signals = SplitSignal(signalInformation);

                    foreach (var s in signals)
                    {
                        if (message == String.Empty)
                        {
                            message = TransformSignal(s);
                        }
                        else
                        {
                            message = message + ";" + TransformSignal(s);
                        }
                    }
                }
                else
                {
                    message = TransformSignal(signalInformation);
                }

                PlaceOrder(message);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "NewSignal");
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HeartbeatTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.DisconnectFromServer();
                Logger.Debug("Connection lost to server", OType.FullName, "HeartbeatTimerElapsed");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "HeartbeatTimerElapsed");
            }
        }

        private void SetSuffixes(string suffixes)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\suffixes.csv";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (File.Create(path))
                {
                }

                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.Write(suffixes);
                Logger.Debug("System Suffixes = " + suffixes, OType.FullName, "SetSuffixes");
                streamWriter.Write(";");

                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SetSuffixes");
            }
        }

        /// <summary>
        /// Saves order into a text file
        /// </summary>
        public void PlaceOrder(string orderInfo)
        {
            try
            {
                lock (this)
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "\\orders.csv";

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    using (File.Create(path))
                    {
                    }

                    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Read);
                    var streamWriter = new StreamWriter(fileStream);

                    streamWriter.Write(orderInfo);
                    Logger.Debug("Signal Information Written to file", OType.FullName, "PlaceOrder");

                    streamWriter.Close();
                    fileStream.Close();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "PlaceOrder");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetTimer()
        {
            try
            {
                _heartbeatTimer.Stop();
                _heartbeatTimer.Start();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ResetTimer");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void FreeResources()
        {
            try
            {
                if (Status == "Connected")
                {
                    this.DisconnectFromServer();
                }
                Dispose(true);
                this._clientTerminal.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "FreeResources");
            }
        }

        private void CommunicationObjectOnClosed(object clientHandler, EventArgs eventArgs)
        {
            try
            {
                DisconnectFromServer();
                _clientTerminal.Close();
                Logger.Debug("Client Unsubscribed because of fault.", OType.FullName, "CommunicationObjectOnClosed");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "CommunicationObjectOnClosed");
            }
        }

        private void SpawnSettingWindow()
        {
            try
            {
                _clientTerminal.Show();
            }
            catch (Exception)
            {
                
            }
        }

        private void CloseSettingsWindow()
        {
            _clientTerminal.Hide();
        }

        private void InitializeClientTerminalUI()
        {
            try
            {
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                        {
                                                                                            IsSystemEnabledChecked = true;
                                                                                            IsAfterHoursChecked = true;
                                                                                            ExitAfterHours = "03:00:00";
                                                                                            IsMnaualExitChecked = false;
                                                                                            IsAfterStopsChecked = true;
                                                                                            IsStopLossChecked = true;
                                                                                            StopLossBalance = "0.01";
                                                                                            IsTakeProfitChecked = true;
                                                                                            TakeProfitBalance = "0.02";
                                                                                            IsMnaualStopsChecked = false;
                                                                                            TotalBalanceAllocation = "0.25";
                                                                                            TradeSizeTypeCollection.Add("Fixed Lots");
                                                                                            TradeSizeTypeCollection.Add("Fixed Cash Risk");
                                                                                            TradeSizeTypeCollection.Add("%age of Balance");
                                                                                            TradeSizeTypeCollection.Add("%age of Equity");
                                                                                            SelectedTradeSizeType = "Fixed Lots";
                                                                                            TradeSizeType = "Enter Number of Lots";
                                                                                            TradeSizeValue = "1.0";
                                                                                            TradeSizeValueVisibility = true;
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
        /// <param name="originalSignal"></param>
        /// <returns></returns>
        private string TransformSignal(string originalSignal)
        {
            string[] splitted = originalSignal.Split(',');

            int exitAfterMinutes;
            if(IsAfterHoursChecked)
            {
                exitAfterMinutes = ParseExitTime(ExitAfterHours);
            }
            else
            {
                exitAfterMinutes = -1;
            }

            decimal takeProfitBalancePercent, stopLossBalancePercent;
            if(IsAfterStopsChecked)
            {
                if (IsStopLossChecked)
                {
                    stopLossBalancePercent = Convert.ToDecimal(StopLossBalance);
                }
                else
                {
                    stopLossBalancePercent = -1;
                }

                if(IsTakeProfitChecked)
                {
                    takeProfitBalancePercent = Convert.ToDecimal(TakeProfitBalance);
                }
                else
                {
                    takeProfitBalancePercent = -1;
                }
            }
            else
            {
                stopLossBalancePercent = -1;
                takeProfitBalancePercent = -1;
            }

            string transformedSignal = splitted[0] + "," + splitted[1] + "," + splitted[2] + "," + exitAfterMinutes +
                                       "," + stopLossBalancePercent + "," + takeProfitBalancePercent;
            Logger.Debug("Transformed Signal = " + transformedSignal, OType.FullName, "TransformSignal");
            return transformedSignal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int ParseExitTime(string time)
        {
            try
            {
                int result = CalculateSeconds(time);
                return result;
            }
            catch (Exception)
            {
                DateTime exitTime = Convert.ToDateTime(time);
                TimeSpan diff = exitTime - DateTime.Today;
                string convertedTime = diff.ToString();

                int result = CalculateSeconds(convertedTime);
                return result;
            }
        }

        private int CalculateSeconds(string time)
        {
            string[] splittedTime = time.Split(':');

            int hours = Convert.ToInt32(splittedTime[0]);
            int minutes = Convert.ToInt32(splittedTime[1]);
            int seconds = Convert.ToInt32(splittedTime[2]);

            int result = hours * 60 * 60 + minutes * 60 + seconds;
            return result;
        }

        private string[] SplitSignal(string signals)
        {
            if (signals.Contains(';'))
            {
                string[] splittedSignals = signals.Split(';');

                return splittedSignals;
            }
            else
            {
                return null;
            }
        }

        public void TradeSizeTypeSelectionChanged()
        {
            try
            {
                this._currentDispatcher.Invoke(DispatcherPriority.Normal, (Action) (() =>
                                                                                        {
                                                                                            if (SelectedTradeSizeType == "Fixed Lots")
                                                                                            {
                                                                                                TradeSizeType = "Number of Lots";
                                                                                            }
                                                                                            else if (SelectedTradeSizeType == "Fixed Cash Risk")
                                                                                            {
                                                                                                TradeSizeType = "Amount to Risk";
                                                                                            }
                                                                                            else if (SelectedTradeSizeType == "%age of Balance")
                                                                                            {
                                                                                                TradeSizeType = "% Balance to Risk";
                                                                                            }
                                                                                            else if (SelectedTradeSizeType == "%age of Equity")
                                                                                            {
                                                                                                TradeSizeType = "% Equity to Risk";
                                                                                            }
                                                                                        }));

            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "TradeSizeTypeSelectionChanged");
            }
        }
    }
}

