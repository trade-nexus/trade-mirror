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
using System.ServiceModel;
using System.Threading;
using System.Timers;
using System.Windows;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    [ServiceContract(Namespace = "http://AutoFXProfitsServer", SessionMode = SessionMode.Required, CallbackContract = typeof(ITradeMirrorClientContract))]
    public interface ITradeMirror
    {
        [OperationContract]
        //bool Subscribe(string userName, string password, int accountID);
        string Subscribe(string userName, string password, int accountID);

        [OperationContract]
        bool Unsubscribe(string userName, string password, int accountID);

        [OperationContract]
        void PublishNewSignal(string signalInformation);
    }

    public interface ITradeMirrorClientContract
    {
        [OperationContract(IsOneWay = true)]
        void NewSignal(string signalInformation);
    }

    public class NewSignalEventArgs : EventArgs
    {
        public string SignalInformation;
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class TradeMirrorService : DependencyObject, ITradeMirror
    {
        private static readonly Type OType = typeof(TradeMirrorService);

        public static event NewSignalEventHandler NewSignalEvent;
        public delegate void NewSignalEventHandler(object sender, NewSignalEventArgs e);

        ITradeMirrorClientContract _callback = null;
        private InstanceContext _communicationObject = null;

        NewSignalEventHandler _newSignalHandler = null;

        private static DBHelper _helper = null;

        private static List<User> AutoFXUsers { get; set; }
        public static List<User> ConnectedUsers { get; set; }

        private ServiceHost _serviceHost = null;

        private static ConnectionManager _connectionManager = new ConnectionManager();

        private int _systemOrderID = 0;

        public int SystemOrderID
        {
            get { return this._systemOrderID; }
            set { this._systemOrderID = value; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TradeMirrorService()
        {
            //if (_helper == null)
            //{
                _helper = new DBHelper(_connectionManager);
            //}
        }

        /// <summary>
        /// Subscribes a user to the signals
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="accountID"> </param>
        /// <returns></returns>
        public string Subscribe(string userName, string password, int accountID)
        {
            Logger.Debug("New Client Connection received. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Subscribe");

            try
            {
                if (SearchHelper.AuthenticateUserCredentials(userName, password, accountID, _helper))
                {
                    Logger.Debug("Client Authenticated. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Subscribe");
                    _callback = OperationContext.Current.GetCallbackChannel<ITradeMirrorClientContract>();
                    _newSignalHandler = new NewSignalEventHandler(NewSignalHandler);

                    _communicationObject = OperationContext.Current.InstanceContext;
                    _communicationObject.Closed += CommunicationObjectOnClosed;
                    _communicationObject.Faulted += CommunicationObjectOnClosed;

                    NewSignalEvent -= _newSignalHandler;
                    NewSignalEvent += _newSignalHandler;
                    //return true;

                    string suffixes = GetSuffixes();
                    Logger.Debug("Suffixes = " + suffixes, OType.FullName, "Subscribe");
                    return suffixes;
                }
                else
                {
                    Logger.Debug("Client Authentication failed. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Subscribe");
                    //return false;

                    return "FAILED";
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Logger.Error(exception, OType.FullName, "Subscribe");
                //return false;
                return "FAILED";
            }
        }

        /// <summary>
        /// Un-Subscribes a user
        /// </summary>
        public bool Unsubscribe(string userName, string password, int accountID)
        {
            try
            {
                if (SearchHelper.UnAuthenticateUserCredentials(userName, password, accountID, _helper))
                {
                    _callback = OperationContext.Current.GetCallbackChannel<ITradeMirrorClientContract>();
                    _newSignalHandler = new NewSignalEventHandler(NewSignalHandler);
                    NewSignalEvent -= _newSignalHandler;
                    Logger.Debug("Client Unsubscribed. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Unsubscribe");
                    return true;
                }
                else
                {
                    Logger.Debug("Client Cant be unsubscirbed. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Subscribe");
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Logger.Error(exception, OType.FullName, "Unsubscribe");
                return false;
            }
        }

        /// <summary>
        /// Fire the new sinal event to the subscribed users
        /// </summary>
        /// <param name="signalInformation"></param>
        public void PublishNewSignal(string signalInformation)
        {
            try
            {
                if (signalInformation.Contains("___autofxtools trademirror___Alive___"))
                {
                    Logger.Debug("Heartbeat. = " + signalInformation, OType.FullName, "PublishNewSignal");
                }
                else
                {
                    Logger.Debug("New Signal Receievd from data source. Signal = " + signalInformation, OType.FullName, "PublishNewSignal");
                    _systemOrderID++;
                    
                    ThreadPool.QueueUserWorkItem(SendSignalNotification, signalInformation);

                    signalInformation = TransformSignalInformation(signalInformation, _systemOrderID);

                    ThreadPool.QueueUserWorkItem(InsertSignalIntoDB, signalInformation);
                }

                var e = new NewSignalEventArgs {SignalInformation = signalInformation};
                NewSignalEvent(this, e);
                Logger.Debug("New Message Published. Message = " + signalInformation, OType.FullName, "PublishNewSignal");                
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "PublishNewSignal");
            }
        }

        /// <summary>
        /// Service host console
        /// </summary>
        public void Start()
        {
            try
            {
                _serviceHost = new ServiceHost(typeof (TradeMirrorService),
                                                          new Uri("net.tcp://localhost:9000/autofxprofits/service"));
                
                // Open the ServiceHost to create listeners and start listening for messages.
                _serviceHost.Open();

                Logger.Debug("Service host started", OType.FullName, "Start");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "Start");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            try
            {
                _serviceHost.Close();
                Logger.Debug("Service host Stopped", OType.FullName, "Stop");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "Stop");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usersList"></param>
        public void UpdateUserList(List<User> usersList)
        {
            AutoFXUsers = usersList;
        }

        /// <summary>
        /// New Signal event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewSignalHandler(object sender, NewSignalEventArgs e)
        {
            _callback.NewSignal(e.SignalInformation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalSignal"></param>
        /// <param name="orderID"></param>
        /// <returns></returns>
        private string TransformSignalInformation(string originalSignal, int orderID)
        {
            //222,manual,OP9476112_S_USDJPY_T_0_L_1.00_P_83.03_sl_0.00_tp_0.00,20110113093101;

            string strategyType = "manual";
            DateTime creationTime = DateTime.UtcNow;

            string newSignal = orderID + "," + strategyType + "," + originalSignal + "," + creationTime.ToString("yyyyMMddHHmmss") + ";";
            Logger.Info("Signal information transformed to = " + newSignal, OType.FullName, "TransformSignalInformation");

            return newSignal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalInfo"></param>
        private void SendSignalNotification(object signalInfo)
        {
            string signalInformation = (string) signalInfo;

            MailingHelper mailingHelper = new MailingHelper(_helper.BuildUsersList());
            mailingHelper.SendEmail(SignalParser.ParseSignal(signalInformation));
        }


        private void InsertSignalIntoDB(object signalInfo)
        {
            string signalInformation = (string) signalInfo;

            _helper.ParseAndInsertData(signalInformation);
        }

        private string GetSuffixes()
        {
            return _helper.GetSuffixes();
        }

        private void CommunicationObjectOnClosed(object clientHandler, EventArgs eventArgs)
        {
            try
            {
                //_callback = OperationContext.Current.GetCallbackChannel<ITradeMirrorClientContract>();                
                _newSignalHandler = new NewSignalEventHandler(NewSignalHandler);
                NewSignalEvent -= _newSignalHandler;
                
                Logger.Debug("Client Unsubscribed because of fault.", OType.FullName, "CommunicationObjectOnClosed");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "CommunicationObjectOnClosed");
            }
        }
    }
}