using StocksApi.Core.Models;

namespace StocksApi.Core.Services
{
    public interface IStocksService
    {
        Task<Result<StockDetail>> GetStockByTicker(string ticker);
        Task<Result<StockPriceHistory>> GetStockPriceHistory(string ticker, string range);
    }
}
