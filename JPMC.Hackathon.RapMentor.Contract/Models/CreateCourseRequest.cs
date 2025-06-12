namespace JPMC.Hackathon.RapMentor.Contract.Models
{
    public class CreateCourseRequest
    {
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string CoursePrompt { get; set; }
        public string Level { get; set; }
        public string Duration { get; set; }
        public string Skills { get; set; }
        public string AdditionalModules { get; set; }
    }
}
