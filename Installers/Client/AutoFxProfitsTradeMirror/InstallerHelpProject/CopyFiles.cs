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
using System.IO;
using System.Windows.Forms;

namespace InstallerHelpProject
{
    public class CopyFiles
    {
        private string AppDataLocation = System.Environment.GetEnvironmentVariable("APPDATA");

        /// <summary>
        /// Specifies the source path and reteives the target path from the string that is passed to it and then calls the Method Copy to 
        /// copy a file from the source to the target
        /// </summary>
        /// <param name="sourcePath"> </param>
        /// <param name="targetPath"> </param>
        public void Run(string sourcePath, string targetPath)
        {
            try
            {
                string sourceDirectory = @sourcePath;
                string targetDirectory = @targetPath + "\\";

                string[] files = Directory.GetFiles(sourceDirectory);
                int numOfFiles = files.Length;

                if (numOfFiles > 0)
                {
                    foreach (string fName in files)
                    {
                        try
                        {
                            string copyFile = Path.GetFileName(fName);
                            string source = @sourceDirectory + copyFile;
                            File.Delete(source);
                        }
                        catch (Exception exception)
                        {
                        }
                    }

                    string[] dirs = Directory.GetDirectories(sourceDirectory);
                    
                    int numOfDirectories = dirs.Length;
                    
                    if (numOfDirectories > 0)
                    {
                        foreach (string dirName in dirs)
                        {
                            if (dirName.Contains("MQL"))
                            {
                                string[] splittedDirName = dirName.Split('\\');

                                string tempDir = splittedDirName[splittedDirName.Length - 1];
                                string sourceDir = @sourceDirectory + "\\" + tempDir;
                                string[] sDirFiles = Directory.GetFiles(sourceDir);
                                
                                if (sDirFiles.Length > 0)
                                {
                                    foreach (string file in sDirFiles)
                                    {
                                        string[] splitFile = file.Split('\\');

                                        string copyFile = Path.GetFileName(file);
                                        string source = sourceDir + "\\" + copyFile;
                                        string target = string.Empty;

                                        if (splitFile[splitFile.Length - 1].Trim() == "AutoFXProfits - Client.mq4")
                                        {
                                            target = @targetDirectory + "experts\\AutoFXProfits - Client.mq4";
                                        }
                                        else if (splitFile[splitFile.Length - 1].Trim() == "Communication Library.dll")
                                        {
                                            target = @targetDirectory + "experts\\libraries\\Communication Library.dll";
                                        }

                                        if(!string.IsNullOrEmpty(target))
                                        {
                                            Copy(source, target);
                                        }
                                    }
                                }
                            }
                            else if (dirName.Contains("Terminal"))
                            {
                                string[] splittedDirName = dirName.Split('\\');

                                string tempDir = splittedDirName[splittedDirName.Length - 1];
                                
                                string sourceDir = @sourceDirectory + "\\" + tempDir;
                                
                                string[] sDirFiles = Directory.GetFiles(sourceDir);
                                
                                if (sDirFiles.Length > 0)
                                {
                                    foreach (string file in sDirFiles)
                                    {
                                        string[] splitFile = file.Split('\\');

                                        string copyFile = Path.GetFileName(file);
                                        string source = sourceDir + "\\" + copyFile;
                                        
                                        string target = AppDataLocation + "\\AutoFX Profits\\AutoFXProfitsClientTerminal\\";
                                        
                                        target = target + splitFile[splitFile.Length - 1].Trim();
                                        if (!string.IsNullOrEmpty(target))
                                        {
                                            Copy(source, target);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                
            }
        }


        /// <summary>
        /// Method Copy copies the file from souce directory to the target directory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public void Copy(string source, string target)
        {
            try
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                }

                File.Copy(source, target);
            }
            catch (Exception exception)
            {
            }
        }
    }

}
