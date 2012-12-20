using System;
using System.ServiceModel;

namespace Microsoft.Samples.NetTcp
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    public class Client : ITradeMirrorCallback
    {
        static void Main()
        {
            // Create a client
            InstanceContext site = new InstanceContext(null, new Client());
            TradeMirrorClient client = new TradeMirrorClient(site);

            if (client.Subscribe("umerazizmalik", "abdulaziz"))
            {
                Console.WriteLine("Subscribed");
                Console.WriteLine();
                Console.WriteLine("Press ENTER to unsubscribe and shut down client");
            }
            else
            {
                Console.WriteLine("Invalid credentials");
            }
            Console.ReadLine();

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalInformation"></param>
        public void NewSignal(string signalInformation)
        {
            Console.WriteLine("Signal Received = " + signalInformation);

            //TransformOrderInformation(signalInformation);
            //PlaceOrder(signalInformation);
        }
    }


}
