namespace StocksApi.Core.Repositories
{
    public interface IUsersRepo
    {
        Task Get(string userId, string password);
        Task Create(object user); // TODO
    }
}
