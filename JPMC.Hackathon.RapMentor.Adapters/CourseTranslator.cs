using JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    internal static class CourseTranslator
    {
        internal static Contract.Models.Course? ToCourseModel(this CourseDataobject course)
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

        internal static CourseDataobject? ToCourseDBObject(this Contract.Models.Course course)
        {
            return new CourseDataobject
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
