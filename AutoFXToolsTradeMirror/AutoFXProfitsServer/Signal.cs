using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFXProfitsServer
{
    public class Signal
    {
        public string Command { get; set; }
        public int Ticket { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public decimal Lots { get; set; }
        public decimal Balance { get; set; }
        public decimal Price { get; set; }
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal ClosePercentage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ticket"></param>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="lots"></param>
        /// <param name="balance"></param>
        /// <param name="price"></param>
        /// <param name="stopLoss"></param>
        /// <param name="takeProfit"></param>
        public Signal(string command, int ticket, string symbol, string type, decimal lots, decimal balance, decimal price, decimal stopLoss, decimal takeProfit)
        {
            Command = command;
            Ticket = ticket;
            Symbol = symbol;
            Type = type;
            Lots = lots;
            Balance = balance;
            Price = price;
            StopLoss = stopLoss;
            TakeProfit = takeProfit;

            ClosePrice = decimal.MinValue;
            ClosePercentage = decimal.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ticket"></param>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="closePrice"></param>
        /// <param name="closePercentage"></param>
        public Signal(string command, int ticket, string symbol, string type, decimal closePrice, decimal closePercentage)
        {
            Command = command;
            Ticket = ticket;
            Symbol = symbol;
            Type = type;
            ClosePrice = closePrice;
            ClosePercentage = closePercentage;

            Lots = decimal.MinValue;
            Balance = decimal.MinValue;
            Price = decimal.MinValue;
            StopLoss = decimal.MinValue;
            TakeProfit = decimal.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ticket"></param>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="closePrice"></param>
        public Signal(string command, int ticket, string symbol, string type, decimal closePrice)
        {
            Command = command;
            Ticket = ticket;
            Symbol = symbol;
            Type = type;
            ClosePrice = closePrice;

            Lots = decimal.MinValue;
            Balance = decimal.MinValue;
            Price = decimal.MinValue;
            StopLoss = decimal.MinValue;
            TakeProfit = decimal.MinValue;
            ClosePercentage = decimal.MinValue;
        }

        public override string ToString()
        {
            return "Command = " + Command + " | Symbol = " + Symbol + " | Type = " + Type +
                   " | Lots = " + Lots + " | Balance = " + Balance + " | Price = " + Price + " | Stop Loss = " +
                   StopLoss + " | Take Profit = " + TakeProfit + " | Close Price = " +
                   ClosePrice + " | Close Percentage = " + ClosePercentage;
        }
    }
}
