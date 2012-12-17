//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IO;
using System.ServiceModel;
using TraceSourceLogger;

namespace Microsoft.ServiceModel.Samples
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.
    //Client implementation code.

    class Client : ISampleContractCallback
    {
        private static Type _oType = typeof(Client);

        static void Main(string[] args)
        {
            try
            {
                InstanceContext site = new InstanceContext(null, new Client());
                SampleContractClient client = new SampleContractClient(site);

                //create a unique callback address so multiple clients can run on one machine
                WSDualHttpBinding binding = (WSDualHttpBinding)client.Endpoint.Binding;

                string clientcallbackaddress = binding.ClientBaseAddress.AbsoluteUri;
                clientcallbackaddress += Guid.NewGuid().ToString();
                binding.ClientBaseAddress = new Uri(clientcallbackaddress);

                //Subscribe.
                Console.WriteLine("Subscribing");
                Logger.Info("Subscribing", _oType.FullName, "Main");
                client.Subscribe();

                //client.PublishPriceChange("HELLO");

                Console.WriteLine();
                Console.WriteLine("Press ENTER to unsubscribe and shut down client");
                Console.ReadLine();

                Console.WriteLine("Unsubscribing");
                Logger.Info("Unsubscribing", _oType.FullName, "Main");
                client.Unsubscribe();

                //Closing the client gracefully closes the connection and cleans up resources
                client.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void PriceChange(string signalInformation)
        {
            Console.WriteLine("Signal Received = " + signalInformation);
            Logger.Info("Signal Received = " + signalInformation, _oType.FullName, "PriceChange");
            PlaceOrder(signalInformation);
        }

        /// <summary>
        /// Saves order into a text file
        /// </summary>
        public static void PlaceOrder(string orderInfo)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\orders.csv";
                FileStream fs = null;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (fs = File.Create(path))
                {
                }

                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None);
                StreamWriter streamWriter = new StreamWriter(fileStream);


                streamWriter.Write(orderInfo);

                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, _oType.FullName, "PlaceOrder");
            }
        }
    }
}

