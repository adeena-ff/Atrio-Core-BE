namespace Atrio.Application.DTOs;

public class MonthlyReportQuery
{
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public int Month { get; set; } = DateTime.UtcNow.Month;
    public Guid? ClassId { get; set; }
}

public class MonthlyReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string? ClassName { get; set; }
    public decimal OverallPercentage { get; set; }
    public IReadOnlyList<StudentMonthlyRowDto> Students { get; set; } = [];
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
