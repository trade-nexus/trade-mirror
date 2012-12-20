//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.NetTcp
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.NetTcp", SessionMode = SessionMode.Required, CallbackContract = typeof(ITradeMirrorClientContract))]
    public interface ITradeMirror
    {
        [OperationContract]
        bool Subscribe(string userName, string password);
        [OperationContract]
        void Unsubscribe();
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

    // Service class which implements the service contract.
    // Added code to write output to the console window
    public class TradeMirrorService : ITradeMirror
    {
        public static event NewSignalEventHandler NewSignalEvent;
        public delegate void NewSignalEventHandler(object sender, NewSignalEventArgs e);

        ITradeMirrorClientContract callback = null;

        NewSignalEventHandler newSignalHandler = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Subscribe(string userName, string password)
        {
            try
            {
                if (userName == "umerazizmalik" && password == "abdulaziz")
                {
                    callback = OperationContext.Current.GetCallbackChannel<ITradeMirrorClientContract>();
                    newSignalHandler = new NewSignalEventHandler(NewSignalHandler);
                    NewSignalEvent += newSignalHandler;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Console.ReadLine();
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unsubscribe()
        {
            try
            {
                NewSignalEvent -= newSignalHandler;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Console.ReadLine();
            }
        }

        //Information source clients call this service operation to report a price change.
        //A price change event is raised. The price change event handlers for each subscriber will execute.

        public void PublishNewSignal(string signalInformation)
        {
            NewSignalEventArgs e = new NewSignalEventArgs();
            e.SignalInformation = signalInformation;
            NewSignalEvent(this, e);
        }

        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the TradeMirrorService type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(TradeMirrorService), new Uri("net.tcp://localhost:9000/servicemodelsamples/service")))
            {
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

        //This event handler runs when a PriceChange event is raised.
        //The client's PriceChange service operation is invoked to provide notification about the price change.

        public void NewSignalHandler(object sender, NewSignalEventArgs e)
        {
            callback.NewSignal(e.SignalInformation);
        }
    }
}
