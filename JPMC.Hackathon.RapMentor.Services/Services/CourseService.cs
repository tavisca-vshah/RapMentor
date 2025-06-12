using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class CourseService : ICourseService
    {
        public async Task<CourseDetail> GetAsync(string id)
        {
            return new CourseDetail
            {
                Id = id
            };
        }
    }
}
