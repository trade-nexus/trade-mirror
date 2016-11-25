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
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
//using TraceSourceLogger;

namespace InstallerHelpProject
{
    class DirFinder
    {
        public DirFinder()
        {
            
        }

        /// <summary>
        /// Method GetInsatalledSoftware reteieves the paths of the MetaTrader softwares by opening up the key in the unintall folder and 
        /// reading Displayname value
        /// </summary>
        /// <summary>
        /// Method GetInstalledSoftware2 retrieves the installation path by reading the name of the key and then reteiving the installation path
        /// </summary>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public void GetInstalledSoftware2(string source)
        {
            try
            {
                var copier = new CopyFiles();

                var files = new List<string>();
                foreach (var d in DriveInfo.GetDrives().Where(x => x.IsReady == true))
                {
                    files.AddRange(GetFiles(d.RootDirectory.FullName, "terminal.exe"));
                }
                
                foreach (var file in files)
                {
                    string path = Path.GetDirectoryName(file);
                    copier.Run(source, path);
                }
            }
            catch (Exception exception)
            {
                
            }
        }

        /// <summary>
        /// Method delete to delete the file from the source directory after detecting it haas been copied to the target directory
        /// </summary>
        /// <param name="target"></param>
        public void Delete(string target)
        {
            try
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                }
            }
            catch (Exception exception)
            {
                
            }
        }

        public static IEnumerable<string> GetFiles(string root, string searchPattern)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
                var path = pending.Pop();
                string[] next = null;
                try
                {
                    next = Directory.GetFiles(path, searchPattern);
                }
                catch { }
                if (next != null && next.Length != 0)
                    foreach (var file in next) yield return file;
                try
                {
                    next = Directory.GetDirectories(path);
                    foreach (var subdir in next) pending.Push(subdir);
                }
                catch { }
            }
        }
    }
}
