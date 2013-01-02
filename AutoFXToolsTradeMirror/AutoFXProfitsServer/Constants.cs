using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFXProfitsServer
{
    public static class Constants
    {
        #region Email Constants

        public const string EmailSubject = "AutoFX Profits Trade Alert";
        public const string EmailHeaderPart1 = "--------------------------------Please Do Not Reply To This Email As No One Will Read It ----------------------------";
        public const string EmailHeaderPart2 = "(If you need to contact us follow instructions at the bottom of this email)";
        public const string Greetings1 = "Hi AutoFXProfits Member,";
        public const string TradeEnter = "I just entered a trade:";
        public const string CurrecnyPair = "Pair: ";
        public const string SignalType = "Type: ";
        public const string EntryPrice = "Entry Price: ";
        public const string StopPrice = "Stop Price:";
        public const string Salutations1 = "Regards,";
        public const string Signee = "Alex";
        public const string Company = "Auto FX Profits";
        public const string Notice =
            "p.s. If you need to contact us, please do not reply to this email, please instead go to our member support site and submit a ticket: http://autofxprofits.zendesk.com";
        public const string TradeModifyPart1 = "I just moved my stop in the ";
        public const string TradeModifyPart2 = " trade to ";
        public const string TradeModifyNote = "Note: We are still in this trade";
        public const string TradePartialClosePart1 = "I just closed ";
        public const string TradePartialClosePart2 = "% of the ";
        public const string TradePartialClosePart3 = " trade at a price of ";
        public const string TradePartialClosePart4 = ", so I am still in a  ";
        public const string TradePartialClosePart5 = "% position in this trade.";
        public const string TradeClosePart1 = "The ";
        public const string TradeClosePart2 = " trade has been closed out of the market at a price of ";

        #endregion
    }
}
