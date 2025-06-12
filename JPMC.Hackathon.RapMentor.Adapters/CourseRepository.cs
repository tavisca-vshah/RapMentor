using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DynamoDBContext _context;

        public CourseRepository(IAmazonDynamoDB dynamoDb)
        {
            _context = new DynamoDBContext(dynamoDb);
        }

        public async Task<Contract.Models.Course> CreateCourseAsync(Contract.Models.Course course)
        {
            var saveObject = course.ToCourseDBObject();
            await _context.SaveAsync(saveObject);
            return course;
        }

        public async Task PublishCourseAsync(string id)
        {
            var course = await _context.LoadAsync<Course>(id);
            if (course != null)
            {
                course.IsPublished = true;
                await _context.SaveAsync(course);
            }
        }

        public async Task<Contract.Models.Course?> GetCourseAsync(string id)
        {
            var courseDbObject = await _context.LoadAsync<Course>(id);
            return courseDbObject.ToCourseModel();
        }
    }
}
