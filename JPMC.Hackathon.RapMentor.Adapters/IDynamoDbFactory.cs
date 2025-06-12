using Amazon.DynamoDBv2;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public interface IDynamoDbFactory
    {
        Task<IAmazonDynamoDB> GetClientAsync(DynamoDbSettings dynamoDbSettings);
    }
}