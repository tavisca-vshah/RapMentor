
using JPMC.Hackathon.RapMentor.Contract.Models;

namespace JPMC.Hackathon.RapMentor.Contract.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDetail> GetAsync(string id);
    }
}
