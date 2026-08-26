using System.ComponentModel.DataAnnotations;

namespace Atrio.Application.DTOs;

public class TeacherDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IReadOnlyList<Guid> AssignedClassIds { get; set; } = [];
}

public class CreateTeacherDto
{
    [Required, MaxLength(150)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(256)] public string Email { get; set; } = string.Empty;
    [Required, MinLength(8), MaxLength(128)] public string Password { get; set; } = string.Empty;
    public IReadOnlyList<Guid> AssignedClassIds { get; set; } = [];
}

public class UpdateTeacherDto
{
    [Required, MaxLength(150)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(256)] public string Email { get; set; } = string.Empty;
    public IReadOnlyList<Guid> AssignedClassIds { get; set; } = [];
    public bool IsActive { get; set; } = true;
}

public class TeacherSearchQuery
{
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
    public string? Department { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
