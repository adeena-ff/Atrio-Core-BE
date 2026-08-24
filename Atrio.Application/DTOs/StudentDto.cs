using System.ComponentModel.DataAnnotations;

namespace Atrio.Application.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string? ClassName { get; set; }
    public bool IsActive { get; set; }
    public decimal AttendancePercentage { get; set; }
}

public class CreateStudentDto
{
    [Required, MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string EnrollmentNumber { get; set; } = string.Empty;

    [Required]
    public Guid ClassId { get; set; }
}

public class UpdateStudentDto : CreateStudentDto
{
    public bool IsActive { get; set; } = true;
}

public class StudentSearchQuery
{
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
}
