using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Timers;
using TraceSourceLogger;

namespace Microsoft.ServiceModel.Samples
{
    public class DataSource : IUpDownSignalsCallback
    {
        private const int PortNumber = 6666;
        private static readonly Type OType = typeof (DataSource);

        private static byte[] Buffer { get; set; }
        private static Socket _socket;

        private static InstanceContext _site;
        private static UpDownSignalsClient _client;
        private static Timer _heartbeatTimer;
        private const int DelaySeconds = 30;

        private const string HeartbeatMessage = "___up down signals trade copier___Alive___";

        /// <summary>
        /// DataSource Main function
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Create a client
            _site = new InstanceContext(null, new DataSource());
            _client = new UpDownSignalsClient(_site);

            _heartbeatTimer = new Timer(DelaySeconds * 1000);
            _heartbeatTimer.Elapsed += HeartbeatTimerElapsed;
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Enabled = true;

            while (true)
            {
                ReadDataFromSocket();
            }
        }

        /// <summary>
        /// Processes the data received from AutoFXToolsSender
        /// </summary>
        private static void ProcessDataReceived(string signalInformation)
        {
            try
            {
                Logger.Debug("New Signal received = " + signalInformation, OType.FullName, "ProcessDataReceived");
                Console.WriteLine("New Signal received = " + signalInformation);

                _client.PublishNewSignal(signalInformation);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception = " + ex.Message, OType.FullName, "ProcessDataReceived");
            }
        }

        /// <summary>
        /// Reads data from socket sent by AutoFXToolsSender
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
                    Logger.Info("Number of bytes received form socket = " + bytesRead, OType.FullName,
                                "ReadDataFromSocket");

                    var formatted = new byte[bytesRead];
                    for (int i = 0; i < bytesRead; i++)
                    {
                        formatted[i] = Buffer[i];
                    }

                    string strData = Encoding.ASCII.GetString(formatted);
                    Logger.Info("Data received form socket = " + strData, OType.FullName, "ReadDataFromSocket");

                    ProcessDataReceived(strData);

                    accpeted.Close();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ReadDataFromSocket");
                _socket.Close();
            }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void HeartbeatTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _client.PublishNewSignal(HeartbeatMessage + DateTime.UtcNow);
        }
    }
}

