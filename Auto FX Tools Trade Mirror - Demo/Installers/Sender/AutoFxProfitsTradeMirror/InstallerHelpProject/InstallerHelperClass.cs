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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.IO;


namespace InstallerHelpProject
{
    [RunInstaller(true)]
    public partial class InstallerHelperClass : System.Configuration.Install.Installer
    {
        private const string EnvironementVariable = "USERPROFILE";
        public InstallerHelperClass()
        {
            InitializeComponent();

            string appDataLocation = System.Environment.GetEnvironmentVariable("APPDATA");
            //MessageBox.Show("App Data Loc = " + appDataLocation);
            Directory.CreateDirectory(appDataLocation + "\\AutoFX Profits Demo");
            Directory.CreateDirectory(appDataLocation + "\\AutoFX Profits Demo\\datasource");
            Directory.CreateDirectory(appDataLocation + "\\AutoFX Profits Demo\\logs");
        }


        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }


        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            string targeted = this.Context.Parameters["targetdir"];
            //string dir = targeted + "InstallerHelpProject.exe";

            DirFinder dr = new DirFinder();
            ////MessageBox.Show("Deleting target: " + dir);

            dr.GetInstalledSoftware2(targeted);    
        }

    }
}
