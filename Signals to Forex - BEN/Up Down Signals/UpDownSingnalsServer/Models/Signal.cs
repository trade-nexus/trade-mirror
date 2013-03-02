namespace UpDownSingnalsServer.Models
{
    public class Signal
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
    }
}
