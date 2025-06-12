using JPMC.Hackathon.RapMentor.Contract.Models;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public interface ICourseRepository
    {
        Task<Course> CreateCourseAsync(Course course);
        Task<Course?> GetCourseAsync(string id);
        Task PublishCourseAsync(string id);
    }
}