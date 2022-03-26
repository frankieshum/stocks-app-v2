namespace StocksApi.Core.Models
{
    public class StockPriceHistory
    {
        public StockPriceHistory(Stock stock)
        {
            Stock = stock;
            PriceHistory = new List<Quote>();
        }

        public Stock Stock { get; init; }
        public List<Quote> PriceHistory { get; set; }
    }
}
