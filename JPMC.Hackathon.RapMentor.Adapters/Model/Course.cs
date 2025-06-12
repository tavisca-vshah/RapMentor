using Amazon.DynamoDBv2.DataModel;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model
{
    [DynamoDBTable("Courses")]
    public class Course
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string CourseStatus { get; set; }
        public string AuthorId { get; set; }
    }

}
