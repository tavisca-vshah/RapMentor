using Amazon;
using Amazon.DynamoDBv2;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public class DynamoDbFactory : IDynamoDbFactory
    {
        public async Task<IAmazonDynamoDB> GetClientAsync(DynamoDbSettings dynamoDbSettings)
        {
            var config = new AmazonDynamoDBConfig();
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(dynamoDbSettings.Region);
            var dbClient = new AmazonDynamoDBClient(config);
            return dbClient;
        }
    }
}
