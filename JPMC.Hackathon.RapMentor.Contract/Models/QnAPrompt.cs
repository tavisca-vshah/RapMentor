using System.Collections.Generic;

namespace JPMC.Hackathon.RapMentor.Contract.Models
{
    public class QnAPrompt
    {
        public List<Prompt> Prompts { get; set; }
    }
    public class Prompt
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
