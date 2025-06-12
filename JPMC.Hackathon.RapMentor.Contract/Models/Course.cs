namespace JPMC.Hackathon.RapMentor.Contract.Models
{
    public class Course
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string AuthorId { get; set; }
        public bool IsPublished { get; set; }
    }


    public class Module
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public List<Lesson> Lessons { get; set; }
    }

    // Models/Lesson.cs
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string ContentUrl { get; set; }
        public int Duration { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
    }


    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
