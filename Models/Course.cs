using System;
using System.ComponentModel.DataAnnotations;

namespace StudentCourseAPI.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

}
