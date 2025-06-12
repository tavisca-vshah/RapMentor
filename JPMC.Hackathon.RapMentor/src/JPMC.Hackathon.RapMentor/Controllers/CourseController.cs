using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JPMC.Hackathon.RapMentor.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        // GET: api/<CourseController>
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] string? authorId, [FromQuery] bool? isPublished)
        {
            var courses = await _courseService.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(authorId))
            {
                courses = courses.Where(x => x.AuthorId.Equals(authorId, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (isPublished.HasValue && isPublished.Value)
            {
                courses = courses.Where(x => x.CourseStatus == CourseStatus.Published.ToString()).ToList();
            }

            return Ok(courses);
        }

        // GET api/<CourseController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var courseDetial = await _courseService.GetAsync(id);
            return Ok(courseDetial);
        }

        // POST api/<CourseController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CourseController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CourseController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
