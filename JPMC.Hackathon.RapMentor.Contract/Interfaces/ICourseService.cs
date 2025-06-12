
using JPMC.Hackathon.RapMentor.Contract.Models;

namespace JPMC.Hackathon.RapMentor.Contract.Interfaces
{
    public interface ICourseService
    {
        Task<Course> GetAsync(string id);
        Task<List<Course>> GetAllAsync();
    }
}
