using JPMC.Hackathon.RapMentor.Adapter.Dynamodb;
using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using JPMC.Hackathon.RapMentor.Services.Adapters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class CourseService : ICourseService
    {
        private string _baseUrl = "https://0jjmybfxv1.execute-api.us-east-1.amazonaws.com/Prod";
        private readonly ICourseRepository _courseRepository;
        private readonly IAmazonLambda _lambda;

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
            var course = await FormulateCourseAsync(request);
            return await _courseRepository.CreateCourseAsync(course);
        }

        public async Task<Course> FormulateCourseAsync(CreateCourseRequest request)
        {
            // 1. Call Lambda 1 to get modules list

            var httpClient = new HttpClient();
            var headingsTask = httpClient.PostAsJsonAsync($"{_baseUrl}/api/courses/headings", request);
            var descriptionTask = httpClient.PostAsJsonAsync($"{_baseUrl}/api/courses/description", request);

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
                    var requestWithHeader = new
                    {
                        AuthorId = request.AuthorId,
                        CoursePrompt = request.CoursePrompt,
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

            }
            return null;
        }

        private static async Task<(string Header, string Content)> FetchContentAsync(HttpClient client, CreateCourseRequest request)
        {
            var response = await client.PostAsJsonAsync($"{_baseUrl}/api/courses/headings/Content", request);
            var content = response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : $"Error: {response.StatusCode}";
            return (request.Header, content);
        }
}

            

        public async Task<Course> UpdateCourseAsync(string courseId, Course updatedCourse)
        {
            // 1. Load existing course
            var existingCourse = await _courseRepository.GetAsync(courseId);
            if (existingCourse == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            // 2. Update fields (you can update only allowed fields)
            existingCourse.Title = updatedCourse.Title ?? existingCourse.Title;
            existingCourse.Modules = updatedCourse.Modules ?? existingCourse.Modules;

            // 3. Save updated course back to DynamoDB
            await _courseRepository.UpdateCourseAsync(existingCourse);

            return existingCourse;
        }

        public async Task<Course> PublishCourseAsync(string courseId)
        {
            await _courseRepository.PublishCourseAsync(courseId);
            var existingCourse = await _courseRepository.GetAsync(courseId);
            if (existingCourse == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            var courseUploader = new CourseUploader("BucketName");
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


    }
}
