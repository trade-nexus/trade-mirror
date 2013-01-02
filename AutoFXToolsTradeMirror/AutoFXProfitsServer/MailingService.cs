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
        public void SendMailNotification(string subject, string mailId, string password, string mailClient, string mailRecipients, string report)
        {
            try
            {
                var message = new MailMessage();

                string[] mailingList = mailRecipients.Split(';');

                foreach (var id in mailingList)
                {
                    message.To.Add(id);
                }

                message.Subject = subject;
                message.From = new MailAddress(mailId);
                message.Body = report;
                var smtp = InitializeSmtpClient(mailClient);

                if (smtp != null)
                {
                    smtp.Credentials = new NetworkCredential(mailId, password);
                    smtp.Send(message);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SendMailNotification");
            }
        }

        /// <summary>
        /// Initialize SMTP Client for the specified Mailing Client
        /// </summary>
        private static SmtpClient InitializeSmtpClient(string mailingClient)
        {
            try
            {
                var smtp = new SmtpClient {Port = 587};
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
