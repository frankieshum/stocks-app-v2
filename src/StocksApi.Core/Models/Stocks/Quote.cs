namespace StocksApi.Core.Models
{
    public record class Quote(decimal price, string currency, DateTime timestamp);
}
