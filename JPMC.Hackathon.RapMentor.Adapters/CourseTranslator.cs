using JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    internal static class CourseTranslator
    {
        internal static Contract.Models.Course? ToCourseModel(this Course course)
        {
            return new Contract.Models.Course
            {
                Category = course.Category,
                Description = course.Description,
                Id = course.Id,
                AuthorId = course.AuthorId,
                Title = course.Title,
                CourseStatus = course.CourseStatus,
            };
        }

        internal static Course? ToCourseDBObject(this Contract.Models.Course course)
        {
            return new Course
            {
                Category = course.Category,
                Description = course.Description,
                Id = course.Id,
                AuthorId = course.AuthorId,
                Title = course.Title,
                CourseStatus = course.CourseStatus,
            };
        }
    }
}
