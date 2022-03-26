namespace StocksApi.Core.Services
{
    public interface ITokenService
    {
        Task GenerateToken(string userId);
    }
}
