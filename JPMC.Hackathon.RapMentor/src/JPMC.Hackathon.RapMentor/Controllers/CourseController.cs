using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JPMC.Hackathon.RapMentor.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAllSync()
        {
            var courses = await _courseService.GetAllAsync();
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
        public async Task<IActionResult> CreateCourseAsync([FromBody] CreateCourseRequest value)
        {
            var courseDetial = await _courseService.CreateCourseAsync(value);
            return Ok(courseDetial);
        }

        // PUT api/<CourseController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourseAsync(string id, [FromBody] Course value)
        {
            var courseDetial = await _courseService.UpdateCourseAsync(id, value);
            return Ok(courseDetial);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PublishCourseAsync(string id)
        {
            var courseDetial = await _courseService.PublishCourseAsync(id);
            return Ok(courseDetial);
        }

        // DELETE api/<CourseController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
