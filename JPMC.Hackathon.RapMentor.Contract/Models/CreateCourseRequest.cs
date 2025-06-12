using System.Collections.Generic;

namespace JPMC.Hackathon.RapMentor.Contract.Models
{
    public class CreateCourseRequest
    {
        public string AuthorId { get; set; }
        public string CoursePrompt { get; set; }
        public string Level { get; set; }
        public string Duration { get; set; }
        public List<string> Skills { get; set; }
        public List<string> AdditionalModules { get; set; }
    }
}
