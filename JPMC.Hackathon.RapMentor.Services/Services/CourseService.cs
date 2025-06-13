using JPMC.Hackathon.RapMentor.Adapter.Dynamodb;
using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using JPMC.Hackathon.RapMentor.Services.Adapters;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class CourseService : ICourseService
    {
        private string _baseUrl = "https://0jjmybfxv1.execute-api.us-east-1.amazonaws.com/Prod";
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

        public async Task<Course> CreateCourseAsync(CreateCourseRequest request)
        {
            var basicCourse = new Course
            {
                AuthorId = request.AuthorId,
                Title = request.Title,
                Duration = request.Duration,
                Level = request.Level,
                CourseStatus = CourseStatus.GeneratingDraft.ToString()
            };
            var courseWithBasicDetial = await _courseRepository.CreateCourseAsync(basicCourse);
            var course = await FormulateCourseAsync(courseWithBasicDetial.Id, request);
            return await _courseRepository.CreateCourseAsync(course);
        }

        public async Task<Course> FormulateCourseAsync(string courseId, CreateCourseRequest request)
        {
            // 1. Call Lambda 1 to get modules list

            var httpClient = new HttpClient();
            var headingsTask = httpClient.PostAsJsonAsync($"{_baseUrl}/api/courses/headings", request);
            var descriptionTask = httpClient.PostAsJsonAsync($"{_baseUrl}/api/courses/summerization", request);

            await Task.WhenAll(headingsTask, descriptionTask);

            var headingsResponse = await headingsTask;
            var descriptionResponse = await descriptionTask;


            if (headingsResponse.IsSuccessStatusCode && descriptionResponse.IsSuccessStatusCode)
            {
                var headings = await headingsResponse.Content.ReadFromJsonAsync<List<string>>();
                var description = await descriptionResponse.Content.ReadAsStringAsync();


                var contentTasks = new List<Task<(string Header, string Content)>>();

                foreach (var heading in headings)
                {
                    var requestWithHeader = new HeaderContentRequest
                    {
                        Prompt = request.CoursePrompt,
                        Level = request.Level,
                        Duration = request.Duration,
                        Skills = request.Skills,
                        AdditionalModules = request.AdditionalModules,
                        Header = heading
                    };


                    var task = FetchContentAsync(httpClient, requestWithHeader);
                    contentTasks.Add(task);
                }

                var results = await Task.WhenAll(contentTasks);
                var module = new List<Module>();
                foreach (var result in results)
                {
                    if (!string.IsNullOrEmpty(result.Content) && !string.IsNullOrEmpty(result.Header) && !result.Content.Contains("Error: InternalServerError"))
                    {
                        module.Add(new Module()
                        {
                            Title = result.Header,
                            Content = result.Content
                        });
                    }
                }

                var course = new Course
                {
                    Id = courseId,
                    Title = request.Title,
                    AuthorId = request.AuthorId,
                    Description = description,
                    Level = request.Level,
                    CourseStatus = CourseStatus.Draft.ToString(),
                    Duration = request.Duration,
                    Modules = module
                };
                return course;

            }
            return null;
        }

        private async Task<(string Header, string Content)> FetchContentAsync(HttpClient client, HeaderContentRequest request)
        {
            var response = await client.PostAsJsonAsync($"{_baseUrl}/api/courses/headings/Content", request);
            var content = response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : $"Error: {response.StatusCode}";
            return (request.Header, content);
        }

        public async Task<Course> UpdateCourseAsync(string courseId, Course updatedCourse)
        {
            // 1. Load existing course
            var existingCourse = await _courseRepository.GetAsync(courseId);
            if (existingCourse == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            // 2. Update fields (you can update only allowed fields)
            existingCourse.Title = updatedCourse.Title ?? existingCourse.Title;
            existingCourse.Description = updatedCourse.Description ?? existingCourse.Description;

            var newModuleIds = updatedCourse.Modules.Select(x => x.Id).ToHashSet();
            var moduleToDelete = existingCourse.Modules.Where(x => !newModuleIds.Contains(x.Id)).Select(x => x.Id).ToList();

            await _courseRepository.DeleteModuleAsync(courseId, moduleToDelete);

            existingCourse.Modules = updatedCourse.Modules ?? existingCourse.Modules;
            // 3. Save updated course back to DynamoDB
            await _courseRepository.UpdateCourseAsync(existingCourse);

            return existingCourse;
        }

        public async Task<Course> PublishCourseAsync(string courseId)
        {
            var existingCourse = await _courseRepository.GetAsync(courseId);
            if (existingCourse == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            await _courseRepository.PublishCourseAsync(courseId, existingCourse);

            var courseUploader = new CourseUploader("rag-input-2025");
            await courseUploader.UploadCourseToS3Async(existingCourse, "courses/docs", $"{courseId}.txt");

            return existingCourse;
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

        public Task<string> GetGoal(GoalRequest request)
        {
            return BedrockKBAdapter.GetRagGoalAsync(request);
        }
    }
}
