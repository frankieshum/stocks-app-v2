namespace StocksApi.Core.Services
{
    public interface IUsersService
    {
        Task Authenticate(string userId, string password);
        Task Register(string userId, string password);
    }
}
