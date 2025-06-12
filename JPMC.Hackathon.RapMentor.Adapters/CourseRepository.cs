using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using JPMC.Hackathon.RapMentor.Contract.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var course = await _context.LoadAsync<Adapter.Dynamodb.Model.Course>(id);
            if (course != null)
            {
                course.CourseStatus = CourseStatus.Published.ToString();
                await _context.SaveAsync(course);
            }
        }

        public async Task<Contract.Models.Course?> GetAsync(string id)
        {
            var courseDbObject = await _context.LoadAsync<Adapter.Dynamodb.Model.Course>(id);
            return courseDbObject.ToCourseModel();
        }

        public Task<List<Contract.Models.Course>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Contract.Models.Course> UpdateCourseAsync(Contract.Models.Course course)
        {
            var saveObject = course.ToCourseDBObject();
            await _context.SaveAsync(saveObject);
            return course;
        }
    }
}
