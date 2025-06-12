using JPMC.Hackathon.RapMentor.Contract.Models;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllAsync();
        Task<Course> CreateCourseAsync(Course course);
        Task<Course?> GetAsync(string id);
        Task PublishCourseAsync(string id);
    }
}