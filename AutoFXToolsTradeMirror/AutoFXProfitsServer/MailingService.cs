using System;
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
