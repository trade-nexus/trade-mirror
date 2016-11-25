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
using System.Net.Mail;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public class MailingService
    {
        private static readonly Type OType = typeof(MailingService);

        /// <summary>
        /// Send Mail Notification to the specified Recipients
        /// </summary>
        public bool SendMailNotification(string subject, string mailId, string password, string mailClient, string mailRecipients, string report, string senderName)
        {
            try
            {
                var message = new MailMessage();

                string[] mailingList = mailRecipients.Split(';');

                foreach (var id in mailingList)
                {
                    if(!String.IsNullOrEmpty(id))
                    {
                        message.Bcc.Add(id);
                    }
                }

                message.Subject = subject;
                message.From = new MailAddress(mailId, senderName);
                message.Body = report;
                var smtp = InitializeSmtpClient(mailClient);

                if (smtp != null)
                {
                    smtp.Credentials = new NetworkCredential(mailId, password);
                    smtp.Send(message);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SendMailNotification");
                return false;
            }
        }

        /// <summary>
        /// Initialize SMTP Client for the specified Mailing Client
        /// </summary>
        private static SmtpClient InitializeSmtpClient(string mailingClient)
        {
            try
            {
                //var smtp = new SmtpClient {Port = 587};
                var smtp = new SmtpClient { Port = 25 };
                if (mailingClient.Equals("GOOGLE"))
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                }
                else if (mailingClient.Equals("YAHOO"))
                {
                    smtp.Host = "smtp.mail.yahoo.com";
                    smtp.EnableSsl = false;
                }
                else if (mailingClient.Equals("AURORA"))
                {
                    smtp.Host = "mail.aurorasolutions.org";
                    smtp.EnableSsl = false;
                }
                else if (mailingClient.Equals("autofxprofits"))
                {
                    smtp.Host = "mail.autofxprofits.com";
                    smtp.EnableSsl = false;
                }

                return smtp;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "InitializeSmtpClient");
                return null;
            }
        }
    }
}
