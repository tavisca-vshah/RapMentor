using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public class DynamoDbContextProvider : IDynamoDbContextProvider
    {
        public IDynamoDBContext GetDynamoDbContext(IAmazonDynamoDB client)
        {
            var dynamoDbContext = new DynamoDBContext(client);
            return dynamoDbContext;
        }
    }
}
