using JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    internal static class CourseTranslator
    {
        internal static Contract.Models.Course? ToCourseModel(this Course course)
        {
            return new Contract.Models.Course
            {
                AuthorId = course.AuthorId,
                Category = course.Category,
                Description = course.Description,
                Id = course.Id,
                IsPublished = course.IsPublished,
                Title = course.Title,
            };
        }

        internal static Course? ToCourseDBObject(this Contract.Models.Course course)
        {
            return new Course
            {
                AuthorId = course.AuthorId,
                Category = course.Category,
                Description = course.Description,
                Id = course.Id,
                IsPublished = course.IsPublished,
                Title = course.Title,
            };
        }
    }
}
