using System.Collections.Generic;

namespace JPMC.Hackathon.RapMentor.Contract.Models
{
    public class Course
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string AuthorId { get; set; }
        public string CourseStatus { get; set; }
        public List<Module> Modules { get; set; } = new();
    }

    public class Module
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public int? Duration { get; set; }
        public string? ImageUrl { get; set; }
    }
}
