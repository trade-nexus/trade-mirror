using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public class MailingHelper
    {
        private static readonly Type OType = typeof(MailingService);

        public const string MailID = "umer_aziz56@yahoo.com";
        public const string Password = "manchesterunited123";
        public const string MailingClient = "YAHOO";

        public List<User> UsersList { get; set; }

        private SearchHelper _searchHelper = null;

        private MailingService _mailingService = new MailingService();

        public MailingHelper(List<User> users)
        {
            UsersList = users;
            _searchHelper = new SearchHelper(UsersList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        public void SendEmail(Signal signal)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(Constants.EmailHeaderPart1).AppendLine();
                stringBuilder.Append(Constants.EmailHeaderPart2).AppendLine();
                stringBuilder.Append(Constants.Greetings1).AppendLine().AppendLine();

                if (signal.Command == "OP")
                {
                    stringBuilder.Append(Constants.TradeEnter).AppendLine();
                    stringBuilder.Append(Constants.CurrecnyPair).Append(signal.Symbol).AppendLine();
                    stringBuilder.Append(Constants.SignalType).Append(signal.Type).AppendLine();
                    stringBuilder.Append(Constants.EntryPrice).Append(signal.Price).AppendLine();
                    stringBuilder.Append(Constants.StopPrice).Append(signal.StopLoss).AppendLine();
                    stringBuilder.Append(Constants.StopPrice).Append(signal.StopLoss).AppendLine().AppendLine();
                }
                else if (signal.Command == "MO")
                {
                    stringBuilder.Append(Constants.TradeModifyPart1).Append(signal.Symbol).Append(Constants.TradeModifyPart2)
                        .Append(signal.StopLoss).AppendLine();
                    stringBuilder.Append(Constants.TradeModifyNote).AppendLine().AppendLine();
                }
                else if (signal.Command == "PL")
                {
                    stringBuilder.Append(Constants.TradePartialClosePart1).Append(signal.ClosePercentage).Append(
                        Constants.TradePartialClosePart2).Append(signal.Symbol).Append(Constants.TradePartialClosePart3).
                        Append(signal.ClosePrice).Append(Constants.TradePartialClosePart4).Append(100 -
                                                                                                  signal.ClosePercentage).
                        Append(Constants.TradePartialClosePart5).AppendLine().AppendLine();
                }
                else if (signal.Command == "CL")
                {
                    stringBuilder.Append(Constants.TradeClosePart1).Append(signal.Symbol).Append(Constants.TradeClosePart2).
                        Append(signal.ClosePrice).AppendLine().AppendLine();
                }

                stringBuilder.Append(Constants.Salutations1).AppendLine();
                stringBuilder.Append(Constants.Signee).AppendLine();
                stringBuilder.Append(Constants.Company).AppendLine().AppendLine();
                stringBuilder.Append(Constants.Notice).AppendLine();

                Logger.Info("Sending Notfocation = " + stringBuilder.ToString(), OType.FullName, "SendEmail");
                _mailingService.SendMailNotification(Constants.EmailSubject, MailID, Password, MailingClient, GetEmailReceipients(), stringBuilder.ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SendEmail");
                throw;
            }
        }

        private string GetEmailReceipients()
        {
            return _searchHelper.GetActiveUserAddresses();
        }
    }
}
