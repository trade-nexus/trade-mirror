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
using System.Linq;
using System.Text;

namespace AutoFXProfitsServer
{
    public class Signal
    {
        public string Command { get; set; }
        public int Ticket { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public decimal Lots { get; set; }
        public decimal Balance { get; set; }
        public decimal Price { get; set; }
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal ClosePercentage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ticket"></param>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="lots"></param>
        /// <param name="balance"></param>
        /// <param name="price"></param>
        /// <param name="stopLoss"></param>
        /// <param name="takeProfit"></param>
        public Signal(string command, int ticket, string symbol, string type, decimal lots, decimal balance, decimal price, decimal stopLoss, decimal takeProfit)
        {
            Command = command;
            Ticket = ticket;
            Symbol = symbol;
            Type = type;
            Lots = lots;
            Balance = balance;
            Price = price;
            StopLoss = stopLoss;
            TakeProfit = takeProfit;

            ClosePrice = decimal.MinValue;
            ClosePercentage = decimal.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ticket"></param>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="closePrice"></param>
        /// <param name="closePercentage"></param>
        public Signal(string command, int ticket, string symbol, string type, decimal closePrice, decimal closePercentage)
        {
            Command = command;
            Ticket = ticket;
            Symbol = symbol;
            Type = type;
            ClosePrice = closePrice;
            ClosePercentage = closePercentage;

            Lots = decimal.MinValue;
            Balance = decimal.MinValue;
            Price = decimal.MinValue;
            StopLoss = decimal.MinValue;
            TakeProfit = decimal.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ticket"></param>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="closePrice"></param>
        public Signal(string command, int ticket, string symbol, string type, decimal closePrice)
        {
            Command = command;
            Ticket = ticket;
            Symbol = symbol;
            Type = type;
            ClosePrice = closePrice;

            Lots = decimal.MinValue;
            Balance = decimal.MinValue;
            Price = decimal.MinValue;
            StopLoss = decimal.MinValue;
            TakeProfit = decimal.MinValue;
            ClosePercentage = decimal.MinValue;
        }

        public override string ToString()
        {
            return "Command = " + Command + " | Symbol = " + Symbol + " | Type = " + Type +
                   " | Lots = " + Lots + " | Balance = " + Balance + " | Price = " + Price + " | Stop Loss = " +
                   StopLoss + " | Take Profit = " + TakeProfit + " | Close Price = " +
                   ClosePrice + " | Close Percentage = " + ClosePercentage;
        }
    }
}
