
using JPMC.Hackathon.RapMentor.Contract.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Contract.Interfaces
{
    public interface ICourseService
    {
        Task<Course> GetAsync(string id);
        Task<List<Course>> GetAllAsync();
        Task<List<string>> GetCourseHeadings(CourseHeadersRequest request);
        Task<string> GetHeaderContent(HeaderContentRequest request);
        Task<string> GetCourseSummerization(CourseHeadersRequest request);
        Task<Course> UpdateCourseAsync(string courseId, Course updatedCourse);
        Task<Course> CreateCourseAsync(CreateCourseRequest request);
        Task<Course> PublishCourseAsync(string courseId);
    }
}
