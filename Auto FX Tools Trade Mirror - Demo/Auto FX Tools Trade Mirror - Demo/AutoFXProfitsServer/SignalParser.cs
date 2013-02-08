using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceSourceLogger;

namespace AutoFXProfitsServer
{
    public static class SignalParser
    {
        private static readonly Type OType = typeof(MailingService);

        /*  Possible Singal templates
         * OP + ticket + "_S_" + symbol + "_T_" + type + "_L_" + + lots + "_B_" + balance + "_P_" + price + "_sl_" + stop loss + "_tp_" + take profit
         * MO + ticket + "_S_" + symbol + "_T_" + type + "_L_" + + lots + "_B_" + balance + "_P_" + price + "_sl_" + stop loss + "_tp_" + take profit
         * PL + ticket + "_S_" + symbol + "_T_" + type + "_CP_" + close price + "_PC_" + close percentage 
         * CL + ticket + "_S_" + symbol + "_T_" + type + "_CP_" + close price
         * DE + ticket + "_S_" + symbol + "_T_" + type + "_CP_" + close price
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newSignal"></param>
        /// <returns></returns>
        public static Signal ParseSignal(string newSignal)
        {
            try
            {
                string command = newSignal.Substring(0, 2);
                string refinedSignal = newSignal.Substring(2);

                string[] tempArray = refinedSignal.Split('_');

                #region Variable Definitions

                int ticket;
                string symbol;
                string type;
                decimal lots;
                decimal balance;
                decimal price;
                decimal stopLoss;
                decimal takeProfit;
                decimal closePrice;
                decimal closePercentage;
                Signal signal;

                #endregion

                ticket = Convert.ToInt32(tempArray[0]);
                symbol = tempArray[2];
                type = tempArray[4];

                if (command == "OP" || command == "MO")
                {
                    lots = Convert.ToDecimal(tempArray[6]);
                    balance = Convert.ToDecimal(tempArray[8]);
                    price = Convert.ToDecimal(tempArray[10]);
                    stopLoss = Convert.ToDecimal(tempArray[12]);
                    takeProfit = Convert.ToDecimal(tempArray[14]);

                    signal = new Signal(command, ticket, symbol, type, lots, balance, price, stopLoss, takeProfit);
                    Logger.Info("New Signal Rceived = " + signal.ToString(), OType.FullName, "ParseSignal");
                    return signal;
                }
                else if (command == "PL")
                {
                    closePrice = Convert.ToDecimal(tempArray[6]);
                    closePercentage = Convert.ToDecimal(tempArray[8]);

                    signal = new Signal(command, ticket, symbol, type, closePrice, closePercentage);
                    Logger.Info("New Signal Rceived = " + signal.ToString(), OType.FullName, "ParseSignal");
                    return signal;
                }
                else if (command == "CL" || command == "DE")
                {
                    closePrice = Convert.ToDecimal(tempArray[6]);

                    signal = new Signal(command, ticket, symbol, type, closePrice);
                    Logger.Info("New Signal Rceived = " + signal.ToString(), OType.FullName, "ParseSignal");
                    return signal;
                }
                else
                {
                    Logger.Debug("Invalid signal command = " + command, OType.FullName, "ParseSignal");
                    return null;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ParseSignal");
                return null;
            }
        }
    }
}
