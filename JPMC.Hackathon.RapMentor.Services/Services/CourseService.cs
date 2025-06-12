using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class CourseService : ICourseService
    {
        public Task<List<Course>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Course> GetAsync(string id)
        {

        }
    }
}
