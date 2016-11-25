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
using System.IO;
using System.Linq;
using System.Text;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public class MailingHelper
    {
        private static readonly Type OType = typeof(MailingService);

        public const string MailID = "trade-alerts@autofxprofits.com";
        public const string Password = "M(qZBrF^g^Cx";
        public const string MailingClient = "autofxprofits";
        public const string SenderName = "Alex Pierce";

        public List<User> UsersList { get; set; }

        private MailingService _mailingService = new MailingService();

        public MailingHelper(List<User> users)
        {
            UsersList = users;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        public bool SendEmail(Signal signal)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                string subject = string.Empty;

                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\Templates\\";

                if (signal.Command == "OP")
                {
                    path = path + "New Trade.txt";
                    subject = Constants.EmailSubjectEntry;
                }
                else if (signal.Command == "MO")
                {
                    path = path + "Stop Move.txt";
                    subject = Constants.EmailSubjectStopAdjustment;
                }
                else if (signal.Command == "PL")
                {
                    path = path + "Partial Close.txt";
                    subject = Constants.EmailSubjectPartial;
                }
                else if (signal.Command == "CL")
                {
                    path = path + "Close Trade.txt";
                    subject = Constants.EmailSubjectExit;
                }

                subject = subject.Replace("<Symbol>", signal.Symbol);

                if (!File.Exists(path))
                {
                    return false;
                }

                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader streamReader = new StreamReader(fs);

                string tempString = string.Empty;
                while ((tempString = streamReader.ReadLine()) != null)
                {
                    stringBuilder.Append(tempString).AppendLine();
                }

                if (signal.Command == "OP")
                {
                    stringBuilder.Replace("<Symbol>", signal.Symbol);
                    string orderType = "";
                    if(signal.Type == "0")
                    {
                        orderType = "Buy";
                    }
                    else if(signal.Type == "1")
                    {
                        orderType = "Sell";
                    }
                    stringBuilder.Replace("<Side>", orderType);
                    stringBuilder.Replace("<Entry Price>", signal.Price.ToString());
                    stringBuilder.Replace("<Stop Price>", signal.StopLoss.ToString());
                }
                else if (signal.Command == "MO")
                {
                    stringBuilder.Replace("<Symbol>", signal.Symbol);
                    stringBuilder.Replace("<Stop Price>", signal.StopLoss.ToString());
                }
                else if (signal.Command == "PL")
                {
                    stringBuilder.Replace("<Close Percentage>", signal.ClosePercentage.ToString());
                    stringBuilder.Replace("<Symbol>", signal.Symbol);
                    stringBuilder.Replace("<Exit Price>", signal.ClosePrice.ToString());
                    stringBuilder.Replace("<Remaining Percentage>", (100 - signal.ClosePercentage).ToString());
                }
                else if (signal.Command == "CL")
                {
                    stringBuilder.Replace("<Symbol>", signal.Symbol);
                    stringBuilder.Replace("<Close Price>", signal.ClosePrice.ToString());
                }

                streamReader.Close();
                fs.Close();

                Logger.Info("Sending Notfocation = " + stringBuilder.ToString(), OType.FullName, "SendEmail");
                return _mailingService.SendMailNotification(subject, MailID, Password, MailingClient, GetEmailReceipients(), stringBuilder.ToString(), SenderName);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SendEmail");
                return false;
            }
        }

        public bool SendEmail(string subject, string receipients, string body)
        {
            try
            {
                Logger.Info("Sending Manual Email = " + body, OType.FullName, "SendEmail");
                return _mailingService.SendMailNotification(subject, MailID, Password, MailingClient, GetEmailReceipients(receipients), body, SenderName);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SendEmail");
                return false;
            }
        }

        public string GetEmailTemplate(string command)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\Templates\\";

                if (command == "OP")
                {
                    path = path + "New Trade.txt";
                }
                else if (command == "MO")
                {
                    path = path + "Stop Move.txt";
                }
                else if (command == "PL")
                {
                    path = path + "Partial Close.txt";
                }
                else if (command == "CL")
                {
                    path = path + "Close Trade.txt";
                }

                if (!File.Exists(path))
                {
                    return null;
                }

                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader streamReader = new StreamReader(fs);

                string tempString = string.Empty;
                while ((tempString = streamReader.ReadLine()) != null)
                {
                    stringBuilder.Append(tempString).AppendLine();
                }

                streamReader.Close();
                fs.Close();

                //Logger.Info("Email Template Changed = " + stringBuilder.ToString(), OType.FullName, "GetEmailTemplate");
                return stringBuilder.ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "GetEmailTemplate");
                return null;
            }
        }

        private string GetEmailReceipients()
        {
            return SearchHelper.GetActiveUserAlternateAddresses(UsersList);
        }

        private string GetEmailReceipients(string receipientType)
        {
            return SearchHelper.GetReceipientsFromType(UsersList, receipientType);
        }

        public void SaveTemplate(string templateName, string newEmailTemplate)
        {
            try
            {
                string command = string.Empty;
                if (templateName == "New")
                {
                    command = "OP";
                }
                else if (templateName == "Modify")
                {
                    command = "MO";
                }
                else if (templateName == "Partial Close")
                {
                    command = "PL";
                }
                else if (templateName == "Exit")
                {
                    command = "CL";
                }

                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\Templates\\";

                if (command == "OP")
                {
                    path = path + "New Trade.txt";
                }
                else if (command == "MO")
                {
                    path = path + "Stop Move.txt";
                }
                else if (command == "PL")
                {
                    path = path + "Partial Close.txt";
                }
                else if (command == "CL")
                {
                    path = path + "Close Trade.txt";
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                FileStream stream = File.Create(path);

                StreamWriter sw = new StreamWriter(stream);
                sw.Write(newEmailTemplate);

                Logger.Info(templateName + " Email Template updated to :" + newEmailTemplate, OType.FullName, "EmailTemplateSelectionChanged");

                sw.Close();
                stream.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "EmailTemplateSelectionChanged");
            }
        }
    }
}
