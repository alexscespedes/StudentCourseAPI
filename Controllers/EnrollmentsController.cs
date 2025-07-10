using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentCourseAPI.Data;
using StudentCourseAPI.Models;

namespace StudentCourseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(AppDbContext context, ILogger<EnrollmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int studentId, int courseId)
        {
            if (!await _context.Students.AnyAsync(s => s.Id == studentId))
                return NotFound($"Student {studentId} not found");

            if (!await _context.Courses.AnyAsync(c => c.Id == courseId))
                return NotFound($"Course {courseId} not found");

            var exists = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (exists)
                return Conflict("Student is already enrolled in this course.");

            var enrollment = new Enrollment { StudentId = studentId, CourseId = courseId };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} enrolled in Course {CourseId}", studentId, courseId);
            return Ok("Enrollment successful");
        }

        [HttpDelete]
        public async Task<IActionResult> Unenroll(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return NotFound("Enrollment not found");

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} unenrolled from Course {CourseId} ", studentId, courseId);
            return Ok("Unenrolled successfully");
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCoursesForStudent(int studentId)
        {
            var courses = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsInCourse(int courseId)
        {
            var students = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .Select(e => e.Student)
                .ToListAsync();

            return Ok(students);
        }
    }
}
