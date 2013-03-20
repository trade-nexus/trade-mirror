using System;

namespace UpDownSingnalsServer.Models
{
    public class Signal : IComparable
    {
        public int ID { get; set; }
        public string Symbol { get; set; }
        public string EntrySide { get; set; }
        public decimal EntryPrice { get; set; }
        public string Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="symbol"></param>
        /// <param name="entrySide"></param>
        /// <param name="entryPrice"></param>
        /// <param name="model"></param>
        public Signal(int id, string symbol, string entrySide, decimal entryPrice, string model)
        {
            ID = id;
            Symbol = symbol;
            EntrySide = entrySide;
            EntryPrice = entryPrice;
            Model = model;
        }

        
        public override string ToString()
        {
            return "ID = " + ID + " | Symbol = " + Symbol + " | Entry Side = " + EntrySide +
                   " | Entry Price = " + EntryPrice + " | Model = " + Model;
        }

        public int CompareTo(object obj)
        {
            try
            {
                Signal signal2 = (Signal)obj;
                if (signal2.EntryPrice == this.EntryPrice)
                {
                    if (signal2.EntrySide == this.EntrySide)
                    {
                        if(signal2.Symbol == this.Symbol)
                        {
                            if (signal2.Model == this.Model)
                            {
                                return 0;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception exception)
            {
                return -1;
            }
        }
    }
}
