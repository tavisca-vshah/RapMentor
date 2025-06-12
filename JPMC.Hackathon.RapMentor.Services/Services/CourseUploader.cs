using Amazon.S3;
using Amazon.S3.Transfer;
using JPMC.Hackathon.RapMentor.Contract.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class CourseUploader
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public CourseUploader(string bucketName)
        {
            _s3Client = new AmazonS3Client();
            _bucketName = bucketName;
        }

        public async Task UploadCourseToS3Async(Course course, string folderName, string fileName)
        {
            // 1. Convert course to formatted string
            string content = CourseToFormattedString(course);

            // 2. Create temp file
            var tempFilePath = Path.GetTempFileName();
            var txtFilePath = Path.ChangeExtension(tempFilePath, ".txt");
            File.Move(tempFilePath, txtFilePath);

            await File.WriteAllTextAsync(txtFilePath, content);

            try
            {
                // 3. Define S3 key with folder and file name
                string s3Key = $"{folderName.TrimEnd('/')}/{fileName}";

                // 4. Upload the file to S3
                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(txtFilePath, _bucketName, s3Key);

                Console.WriteLine($"Course uploaded to S3 at '{s3Key}'");
            }
            finally
            {
                // 5. Clean up temp file
                if (File.Exists(txtFilePath))
                {
                    File.Delete(txtFilePath);
                }
            }
        }

        private string CourseToFormattedString(Course course)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Course Details");
            sb.AppendLine("==============");
            sb.AppendLine($"Id: {course.Id}");
            sb.AppendLine($"Title: {course.Title}");
            sb.AppendLine($"Description: {course.Description}");
            sb.AppendLine($"Category: {course.Level}");
            sb.AppendLine($"AuthorId: {course.AuthorId}");
            sb.AppendLine($"CourseStatus: {course.CourseStatus}");
            sb.AppendLine();

            sb.AppendLine("Modules");
            sb.AppendLine("-------");

            if (course.Modules == null || course.Modules.Count == 0)
            {
                sb.AppendLine("No modules available.");
            }
            else
            {
                foreach (var module in course.Modules)
                {
                    sb.AppendLine($"Module Id: {module.Id}");
                    sb.AppendLine($"Title: {module.Title}");
                    sb.AppendLine($"Summary: {module.Summary}");
                    sb.AppendLine($"Content: {module.Content}");
                    sb.AppendLine($"Duration: {(module.Duration.HasValue ? module.Duration.Value.ToString() : "N/A")}");
                    sb.AppendLine($"ImageUrl: {module.ImageUrl ?? "N/A"}");
                    sb.AppendLine(new string('-', 20));
                }
            }

            return sb.ToString();
        }
    }
}
