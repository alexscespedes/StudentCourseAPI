using System;

namespace StudentCourseAPI.Models;

public class Enrollment
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = default!;

}
