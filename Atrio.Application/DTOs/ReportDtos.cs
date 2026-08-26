using Atrio.Application.Common.Models;

namespace Atrio.Application.DTOs;

public class MonthlyReportQuery
{
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public int Month { get; set; } = DateTime.UtcNow.Month;
    public Guid? ClassId { get; set; }
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? TeacherId { get; set; }
}

public class MonthlyReportDto : PagedResponse<StudentMonthlyRowDto>
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string? ClassName { get; set; }
    public decimal OverallPercentage { get; set; }
    // Kept for existing report consumers; Items is the canonical paginated collection.
    public List<StudentMonthlyRowDto> Students
    {
        get => Items;
        set => Items = value;
    }
}

public class StudentMonthlyRowDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Excused { get; set; }
    public decimal Percentage { get; set; }
}
