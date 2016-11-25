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
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Timers;
using TraceSourceLogger;

namespace Microsoft.ServiceModel.Samples
{
    public class DataSource : ITradeMirrorCallback
    {
        private const int PortNumber = 6677;
        private static readonly Type OType = typeof (DataSource);

        private static byte[] Buffer { get; set; }
        private static Socket _socket;

        private static InstanceContext _site;
        private static TradeMirrorClient _client;
        private static Timer _heartbeatTimer;
        private const int DelaySeconds = 30;

        private const string HeartbeatMessage = "___autofxtools trademirror___Alive___";

        /// <summary>
        /// DataSource Main function
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Create a client
            _site = new InstanceContext(null, new DataSource());
            _client = new TradeMirrorClient(_site);

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

