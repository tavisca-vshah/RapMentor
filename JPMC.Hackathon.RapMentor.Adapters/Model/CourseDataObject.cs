using Amazon.DynamoDBv2.DataModel;
using System;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model
{
    [DynamoDBTable("Courses")]
    public class CourseDataobject
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [DynamoDBProperty]
        public string Title { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
        [DynamoDBProperty]
        public string Category { get; set; }
        [DynamoDBProperty]
        public string CourseStatus { get; set; }
        [DynamoDBProperty]
        public string AuthorId { get; set; }
    }

}
