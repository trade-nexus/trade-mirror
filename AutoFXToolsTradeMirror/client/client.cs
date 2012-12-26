using System;
using System.IO;
using System.ServiceModel;
using TraceSourceLogger;

namespace Microsoft.Samples.NetTcp
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    public class Client : ITradeMirrorCallback
    {
        private static readonly Type OType = typeof(Client);

        static void Main()
        {
            // Create a client
            InstanceContext site = new InstanceContext(null, new Client());
            TradeMirrorClient client = new TradeMirrorClient(site);

            if (client.Subscribe("1413684", "forexsuccess", 1413684))
            {
                Logger.Info("Subscribed", OType.FullName, "Main");
                Console.WriteLine("Subscribed");
                Console.WriteLine();
                Console.WriteLine("Press ENTER to unsubscribe and shut down client");
            }
            else
            {
                Logger.Info("Invalid Credentials", OType.FullName, "Main");
                Console.WriteLine("Invalid credentials");
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();

            client.Unsubscribe("1413684", "forexsuccess", 1413684);
            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalInformation"></param>
        public void NewSignal(string signalInformation)
        {
            Console.WriteLine("Signal Received = " + signalInformation);

            if (signalInformation.Contains("___autofxtools trademirror___Alive___"))
            {
                Logger.Debug("Heartbeat. = " + signalInformation, OType.FullName, "PublishNewSignal");
                return;
            }
 
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
                Logger.Error(exception, OType.FullName, "PlaceOrder");
            }
        }
    }


}
