using JPMC.Hackathon.RapMentor.Contract.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllAsync();
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> GetAsync(string id);
        Task PublishCourseAsync(string id, Course course);
        Task<Course> UpdateCourseAsync(Course course);
    }
}