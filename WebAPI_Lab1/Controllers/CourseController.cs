using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Lab1.Data;
using WebAPI_Lab1.Models;

namespace WebAPI_Lab1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CourseController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllCourse()
        {
            var list = _db.Courses.ToList();

            if (list == null)
                return NotFound();

            return Ok(list);
        }

        [HttpGet("api/GetCourseById{id}")]
        public IActionResult GetCourseById([FromForm] int id)
        {
            if (id == null)
                return NotFound();

            var list = _db.Courses.Find(id);

            if (list == null)
                return NotFound();

            return Ok(list);
        }

        [HttpGet("api/GetCourseByName{name}")]
        public IActionResult GetCourseByName([FromForm] string name)
        {
            if (name == null)
                return NotFound();

            var courseName = _db.Courses.FirstOrDefault(c => c.Name.Contains(name));

            if (courseName == null)
                return NotFound();

            return Ok(courseName);
        }

        [HttpPost]
        public IActionResult AddCourse([FromForm] Course course)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _db.Courses.Add(course);
            _db.SaveChanges();

            return Created();
        }

        [HttpDelete("api/DeleteCourse/{id}")]
        public IActionResult DeleteCourse([FromForm] int id)
        {
            if (id == null)
                return NotFound();

            var course = _db.Courses.FirstOrDefault(c => c.Id == id);

            _db.Remove(course);
            _db.SaveChanges();

            return Ok();
        }

        [HttpPut("api/UpdateCourse/{id}")]
        public async Task<IActionResult> UpdateCourse(Course course, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

           Course crs = await _db.Courses.SingleOrDefaultAsync(course => course.Id == id);

            if (crs == null)
                return NotFound();
            else
                _db.Entry(course).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _db.SaveChangesAsync();

            return NoContent();
        }

    }
}
