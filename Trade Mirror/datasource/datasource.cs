using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using TraceSourceLogger;

namespace Microsoft.ServiceModel.Samples
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //DataSource implementation code.

    public class DataSource : ISampleContractCallback
    {
        private const int PortNumber = 6666;
        private static Type _oType = typeof(DataSource);

        private static byte[] Buffer { get; set; }
        private static Socket _socket;

        private static InstanceContext _site;
        private static SampleContractClient _client;

        public static void Main(string[] args)
        {
            _site = new InstanceContext(new DataSource());
            _client = new SampleContractClient(_site);

            while(true)
            {
                ReadDataFromSocket();
            }
            
        }

        /// <summary>
        /// Processes the data received
        /// </summary>
        private static void ProcessDataReceived(string signalInformation)
        {
            try
            {
                Logger.Debug("New Signal received = " + signalInformation, _oType.FullName, "ProcessDataReceived");

                _client.PublishPriceChange(signalInformation);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception = " + ex.Message, _oType.FullName, "ProcessDataReceived");
            }
        }

        /// <summary>
        /// Reads data from socket
        /// </summary>
        public static void ReadDataFromSocket()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _socket.Bind(new IPEndPoint(0, PortNumber));
                _socket.Listen(100);

                while (true)
                {
                    Socket accpeted = _socket.Accept();
                    Buffer = new byte[accpeted.SendBufferSize];
                    int bytesRead = accpeted.Receive(Buffer);
                    Logger.Info("Number of bytes received form socket = " + bytesRead, _oType.FullName, "ReadDataFromSocket");

                    byte[] formatted = new byte[bytesRead];
                    for (int i = 0; i < bytesRead; i++)
                    {
                        formatted[i] = Buffer[i];
                    }

                    string strData = Encoding.ASCII.GetString(formatted);
                    Logger.Info("Data received form socket = " + strData, _oType.FullName, "ReadDataFromSocket");

                    ProcessDataReceived(strData);

                    accpeted.Close();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, _oType.FullName, "ReadDataFromSocket");
                _socket.Close();
            }

        }

        public void PriceChange(string signalInformation)
        {
            Console.WriteLine("Signal Received = " + signalInformation);
        }
    }
}

