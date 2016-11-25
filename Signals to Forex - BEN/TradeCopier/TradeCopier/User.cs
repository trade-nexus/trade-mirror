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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TradeCopier
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        public int Id
        {
            get { return _id; }
            set { if (value != _id) { _id = value; OnPropertyChanged("Id"); } }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { { if (value != _userName) { _userName = value; OnPropertyChanged("UserName"); } } }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { { if (value != _password) { _password = value; OnPropertyChanged("Password"); } } }
        }

        private DateTime _created;
        public DateTime Created
        {
            get { return _created; }
            set { { if (value != _created) { _created = value; OnPropertyChanged("Created"); } } }
        }

        private bool _status;
        public bool Status
        {
            get { return _status; }
            set { { if (value != _status) { _status = value; OnPropertyChanged("Status"); } } }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { { if (value != _email) { _email = value; OnPropertyChanged("Email"); } } }
        }
        private void OnPropertyChanged(String stringName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(stringName));
            }
        }

    }
}
