using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Contract.Models
{
    public class CreateCourseRequest
    {
        public string AuthorId { get; set; }
        public string UserPrompt { get; set; }
        public string Level { get; set; }
        public string Duration { get; set; }
        public List<string> Skills { get; set; }
        public List<string> AdditionalModules { get; set; }
    }
}
