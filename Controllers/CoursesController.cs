using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentCourseAPI.Data;
using StudentCourseAPI.Models;

namespace StudentCourseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(AppDbContext context, ILogger<CoursesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<Course>>> GetAll()
        {
            return await _context.Courses.ToListAsync();
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<Course>> GetById(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<Course>> Create(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Course created: {Course}", course.Title);
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Course course)
        {
            if (id != course.Id) return NotFound();

            var exists = await _context.Courses.AnyAsync(c => c.Id == id);
            if (!exists) return NotFound();

            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
