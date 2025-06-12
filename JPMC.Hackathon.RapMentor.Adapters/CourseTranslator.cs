using JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    internal static class CourseTranslator
    {
        internal static Contract.Models.Course? ToCourseModel(this CourseDataobject course)
        {
            return new Contract.Models.Course
            {
                Level = course.Category,
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
                Category = course.Level,
                Description = course.Description,
                Id = course.Id,
                AuthorId = course.AuthorId,
                Title = course.Title,
                CourseStatus = course.CourseStatus,
            };
        }

        internal static ModuleDataObject? ToModuleObject(this Contract.Models.Module module, string courseId)
        {
            return new ModuleDataObject
            {
                Content = module.Content,
                CourseId = courseId,
                ModuleId = module.Id,
                Summary = module.Summary,
                Title = module.Title,
                Duration = module.Duration,
            };
        }

        internal static Contract.Models.Module? ToModuleModel(this ModuleDataObject module)
        {
            return new Contract.Models.Module
            {
                Content = module.Content,
                Id = module.ModuleId,
                Summary = module.Summary,
                Title = module.Title,
                Duration = module.Duration,
            };
        }
    }
}
