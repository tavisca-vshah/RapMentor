using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public interface IDynamoDbContextProvider
    {
        IDynamoDBContext GetDynamoDbContext(IAmazonDynamoDB client);
    }
}
