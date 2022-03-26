namespace StocksApi.Core.Models
{
    public record class StockDetail(string ticker, string name, Quote quote) : Stock(ticker, name);
}
