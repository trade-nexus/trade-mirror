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

namespace UpDownSingnalsServer.Models
{
    public class Signal : IComparable
    {
        public int ID { get; set; }
        public string Symbol { get; set; }
        public string EntrySide { get; set; }
        public decimal EntryPrice { get; set; }
        public string Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="symbol"></param>
        /// <param name="entrySide"></param>
        /// <param name="entryPrice"></param>
        /// <param name="model"></param>
        public Signal(int id, string symbol, string entrySide, decimal entryPrice, string model)
        {
            ID = id;
            Symbol = symbol;
            EntrySide = entrySide;
            EntryPrice = entryPrice;
            Model = model;
        }

        
        public override string ToString()
        {
            return "ID = " + ID + " | Symbol = " + Symbol + " | Entry Side = " + EntrySide +
                   " | Entry Price = " + EntryPrice + " | Model = " + Model;
        }

        public int CompareTo(object obj)
        {
            try
            {
                Signal signal2 = (Signal)obj;
                if (signal2.EntryPrice == this.EntryPrice)
                {
                    if (signal2.EntrySide == this.EntrySide)
                    {
                        if(signal2.Symbol == this.Symbol)
                        {
                            if (signal2.Model == this.Model)
                            {
                                return 0;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception exception)
            {
                return -1;
            }
        }
    }
}
