using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JPMC.Hackathon.RapMentor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QnAController : ControllerBase
    {
        private readonly IQnAService _qnAService;

        public QnAController(IQnAService qnAService)
        {
            _qnAService = qnAService;
        }

        // GET: api/<QnAController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<QnAController>
        [HttpPost]
        public async Task<string> Post([FromBody] QnAPrompt prompt)
        {
            return await _qnAService.GetRagQnAAsync(prompt);
        }

    }
}
