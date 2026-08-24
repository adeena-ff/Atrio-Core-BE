namespace Atrio.API.DTOs;

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
}

public class CreateStudentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
}
