namespace JPMC.Hackathon.RapMentor.Contract.Models
{
    public class CourseHeadersRequest
    {
        public string Prompt { get; set; }
        public string Skills { get; set; }
        public string Level { get; set; }
        public string Duration { get; set; }
        public string AdditionalModules { get; set; }
    }
}
