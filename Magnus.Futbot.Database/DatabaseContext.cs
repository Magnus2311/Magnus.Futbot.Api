using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Magnus.Futbot.Database
{
    public class DatabaseContext
    {
        protected readonly MongoClient _client;
        protected readonly IMongoDatabase _db;
        private readonly IConfiguration _configuration;

        public DatabaseContext(IConfiguration configuration)
        {
            _configuration = configuration;

            _client = new MongoClient($"mongodb+srv://{_configuration["DB:Futbot:Username"]}:{_configuration["DB:Futbot:Password"]}@{_configuration["DB:Futbot:Cluster"]}.rdkdn.mongodb.net/{_configuration["DB:Futbot:Collection"]}?retryWrites=true&w=majority");

            _db = _client.GetDatabase(_configuration["DB:Futbot:Collection"]);
        }
    }
}