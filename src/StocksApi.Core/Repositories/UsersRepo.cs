using StocksApi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;

namespace StocksApi.Core.Repositories
{
    public class UsersRepo : IUsersRepo
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;

        public UsersRepo(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public Task Create(object user)
        {
            throw new NotImplementedException();
        }

        public Task Get(string userId, string password)
        {

            throw new NotImplementedException();
        }
    }
}
