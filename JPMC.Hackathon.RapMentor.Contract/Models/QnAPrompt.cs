using System.Collections.Generic;

namespace JPMC.Hackathon.RapMentor.Contract.Models
{
    public class QnAPrompt
    {
        public List<Prompt> Prompts { get; set; }
    }
    public class Prompt
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
