using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Timers;
using TraceSourceLogger;

namespace Microsoft.Samples.NetTcp
{
    [ServiceContract(Namespace = "http://Microsoft.Samples.NetTcp", SessionMode = SessionMode.Required, CallbackContract = typeof(ITradeMirrorClientContract))]
    public interface ITradeMirror
    {
        [OperationContract]
        bool Subscribe(string userName, string password, int accountID);

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

    public class TradeMirrorService : ITradeMirror
    {
        private static readonly Type OType = typeof(TradeMirrorService);

        public static event NewSignalEventHandler NewSignalEvent;
        public delegate void NewSignalEventHandler(object sender, NewSignalEventArgs e);

        ITradeMirrorClientContract _callback = null;

        NewSignalEventHandler _newSignalHandler = null;

        private static DBHelper _helper = null;

        private static List<User> AutoFXUsers { get; set; }

        private static Timer _updateUserListTimer;
        private const int UpdateUserListSeconds = 86400;

        private int _systemOrderID = 0;

        public int SystemOrderID
        {
            get { return this._systemOrderID; }
            set { this._systemOrderID = value; }
        }

        /// <summary>
        /// Subscribes a user to the signals
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="accountID"> </param>
        /// <returns></returns>
        public bool Subscribe(string userName, string password, int accountID)
        {
            Logger.Debug("New Client Connection received. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Subscribe");

            try
            {
                
                if (AuthenticateUserCredentials(userName, password, accountID))
                {
                    Logger.Debug("Client Authenticated. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Subscribe");
                    _callback = OperationContext.Current.GetCallbackChannel<ITradeMirrorClientContract>();
                    _newSignalHandler = new NewSignalEventHandler(NewSignalHandler);
                    NewSignalEvent += _newSignalHandler;
                    //ToDO: Add to active users list
                    return true;
                }
                else
                {
                    Logger.Debug("Client Authentication failed. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Subscribe");
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Logger.Error(exception, OType.FullName, "Subscribe");
                return false;
            }
        }

        /// <summary>
        /// Un-Subscribes a user
        /// </summary>
        public bool Unsubscribe(string userName, string password, int accountID)
        {
            try
            {
                NewSignalEvent -= _newSignalHandler;
                Logger.Debug("Client Unsubscribed. UserName = " + userName + " | Password = " + password + " | AccountID = " + accountID, OType.FullName, "Unsubscribe");
                return true;
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

                    signalInformation = TransformSignalInformation(signalInformation, _systemOrderID);
                }

                var e = new NewSignalEventArgs {SignalInformation = signalInformation};
                NewSignalEvent(this, e);
                Logger.Debug("New Message Published. Message = " + signalInformation, OType.FullName, "PublishNewSignal");

                //_helper.ParseAndInsertData(signalInformation);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "PublishNewSignal");
            }
        }

        /// <summary>
        /// Service host console
        /// </summary>
        public static void Main()
        {
            var connectionManager = new ConnectionManager();
            _helper = new DBHelper(connectionManager);
            AutoFXUsers = _helper.BuildUsersList();

            _updateUserListTimer = new Timer(UpdateUserListSeconds * 1000);
            _updateUserListTimer.Elapsed += UpdateUserListTimerElapsed;
            _updateUserListTimer.AutoReset = true;
            _updateUserListTimer.Enabled = true;

            //TEST
            //AuthenticateUserCredentials("1413684", "forexsuccess", 1413684);

            ServiceHost serviceHost = null;
            try
            {
                serviceHost = new ServiceHost(typeof (TradeMirrorService),
                                                          new Uri("net.tcp://localhost:9000/servicemodelsamples/service"));
                
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();
                Logger.Debug("Service host started", OType.FullName, "Main");

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Type 'quit' to terminate service.");

                if (Console.ReadLine() == "quit")
                {
                    serviceHost.Close();
                }
                else
                {
                    Console.ReadLine();
                    serviceHost.Close();
                }
                
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "Main");
            }
            finally
            {
                if (serviceHost != null)
                    ((IDisposable)serviceHost).Dispose();
            }
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

        public static bool AuthenticateUserCredentials(string userName, string password, int accountID)
        {
            try
            {
                User testUser = new User(Convert.ToInt32(userName), Convert.ToInt32(userName), password);
                if (AutoFXUsers.BinarySearch(testUser) > -1)
                {
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

        private string TransformSignalInformation(string originalSignal, int orderID)
        {
            //222,manual,OP9476112_S_USDJPY_T_0_L_1.00_P_83.03_sl_0.00_tp_0.00,20110113093101;

            string strategyType = "manual";
            DateTime creationTime = DateTime.UtcNow;

            string newSignal = orderID + "," + strategyType + "," + originalSignal + "," + creationTime.ToString("yyyyMMddHHmmss") + ";";
            Logger.Info("Signal information transformed to = " + newSignal, OType.FullName, "TransformSignalInformation");

            return newSignal;
        }

        public static void UpdateUserListTimerElapsed(object sender, ElapsedEventArgs e)
        {
            AutoFXUsers = _helper.BuildUsersList();
            Logger.Debug("User List Updated. Count = " + AutoFXUsers.Count, OType.FullName, "UpdateUserListTimerElapsed");
        }
    }
}