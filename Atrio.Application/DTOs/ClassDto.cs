using System.ComponentModel.DataAnnotations;

namespace Atrio.Application.DTOs;

public class ClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int StudentCount { get; set; }
}

public class CreateClassDto
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string AcademicYear { get; set; } = string.Empty;
}

public class UpdateClassDto : CreateClassDto
{
    public bool IsActive { get; set; } = true;
}

public class AssignStudentDto
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid ClassId { get; set; }
}

public class ClassSearchQuery
{
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
    public string? Department { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? TeacherId { get; set; }
}
