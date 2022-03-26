namespace StocksApi.Core.Services
{
    public interface IPortfoliosService
    {
        Task GetAllPortfolios(string userId);
        Task GetPortfolioByName(string portfolioName, string userId);
        Task CreatePortfolio(object portfolio, string userId);
        Task UpdatePortfolio(object portfolio, string userId);
        Task DeletePortfolio(string portfolioName, string userId);
    }
}
