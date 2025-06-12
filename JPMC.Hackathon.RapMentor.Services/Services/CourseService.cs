using JPMC.Hackathon.RapMentor.Adapter.Dynamodb;
using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using JPMC.Hackathon.RapMentor.Services.Adapters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            this._courseRepository = courseRepository;
        }
        public async Task<List<Course>> GetAllAsync()
        {
            return await _courseRepository.GetAllAsync();
        }

        public async Task<Course> GetAsync(string id)
        {
            return await _courseRepository.GetAsync(id);
        }

        public Task<List<string>> GetCourseHeadings(CourseHeadersRequest request)
        {
            return BedrockKBAdapter.GetCourseHeadings(request);
        }

        public Task<string> GetCourseSummerization(CourseHeadersRequest request)
        {
            return BedrockKBAdapter.GetCourseSummerization(request);
        }

        public Task<string> GetHeaderContent(HeaderContentRequest request)
        {
            return BedrockKBAdapter.Getheadercontent(request);
        }


    }
}
