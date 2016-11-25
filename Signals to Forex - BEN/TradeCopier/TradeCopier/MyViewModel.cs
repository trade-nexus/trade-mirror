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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TradeCopier
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<User> _items;
        public ObservableCollection<User> Items
        {
            get { return _items; }
            set { { if (value != _items) { _items = value; OnPropertyChanged("Items"); } } }
        }
        public User CurrentUser { get; set; }
        private void OnPropertyChanged(string items)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(items));
            }
        }

        public MyViewModel()
        {
            //Initilzation of Traders List
            Items = new ObservableCollection<User>();
            Items.Add(new User { Id = 1, Created = DateTime.Now, Password = "abc12@134", Status = true, UserName = "Trader 1", Email = "abc@gmail.com" });
            Items.Add(new User { Id = 2, Created = DateTime.Now, Password = "add12-%@134", Status = true, UserName = "Trader 2", Email = "a77bc@gmail.com" });
            Items.Add(new User { Id = 3, Created = DateTime.Now, Password = "add12%@34", Status = false, UserName = "Trader 3", Email = "ab212c@gmail.com" });
            Items.Add(new User { Id = 4, Created = DateTime.Now, Password = "add12-%34", Status = true, UserName = "Trader 4", Email = "a2321bc@gmail.com" });


        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
