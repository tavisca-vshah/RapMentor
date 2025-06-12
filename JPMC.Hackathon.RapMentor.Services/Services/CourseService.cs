using Amazon.Lambda;
using Amazon.Lambda.Model;
using JPMC.Hackathon.RapMentor.Adapter.Dynamodb;
using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using JPMC.Hackathon.RapMentor.Services.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class CourseService : ICourseService
    {
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
            var modulesPayload = JsonSerializer.Serialize(new
            {
                userPrompt = request.UserPrompt,
                level = request.Level,
                additionalModules = request.AdditionalModules
            });

            var listModulesResponse = await _lambda.InvokeAsync(new InvokeRequest
            {
                FunctionName = "list-modules-lambda", // Replace with your Lambda name or ARN
                Payload = modulesPayload
            });

            var modulesJson = Encoding.UTF8.GetString(listModulesResponse.Payload.ToArray());
            var moduleNames = JsonSerializer.Deserialize<List<string>>(modulesJson);

            if (moduleNames == null || moduleNames.Count == 0)
            {
                throw new Exception("No modules returned from module listing Lambda.");
            }

            // 2. Call Lambda 2 in parallel to generate content for each module
            var contentTasks = moduleNames.Select(async moduleName =>
            {
                var contentPayload = JsonSerializer.Serialize(new
                {
                    moduleName = moduleName,
                    userPrompt = request.UserPrompt,
                    level = request.Level,
                    duration = request.Duration,
                    skills = request.Skills
                });

                var contentResponse = await _lambda.InvokeAsync(new InvokeRequest
                {
                    FunctionName = "generate-module-content-lambda", // Replace with your Lambda name or ARN
                    Payload = contentPayload
                });

                var contentJson = Encoding.UTF8.GetString(contentResponse.Payload.ToArray());
                return JsonSerializer.Deserialize<Module>(contentJson);
            });

            var moduleContents = await Task.WhenAll(contentTasks);

            // 3. Combine results into a course
            var course = new Course
            {
                Title = request.UserPrompt,
                Modules = moduleContents.ToList(),
                AuthorId = request.AuthorId,
                CourseStatus = CourseStatus.Draft.ToString() // default to draft on creation
            };

            return course;
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
