namespace Atrio.Application.DTOs;

public class DashboardAnalyticsDto
{
    public DashboardAnalyticsKpisDto Kpis { get; set; } = new();
    public IReadOnlyList<DailyTrendDto> DailyTrends { get; set; } = [];
    public IReadOnlyList<CourseRateDto> CourseRates { get; set; } = [];
    public IReadOnlyList<AtRiskStudentDto> AtRiskStudents { get; set; } = [];
}

public class DashboardAnalyticsKpisDto
{
    public int TotalStudents { get; set; }
    public int TotalClasses { get; set; }
    public decimal AverageAttendanceRate { get; set; }
    public int AtRiskCount { get; set; }
}

public class DailyTrendDto
{
    public DateOnly Date { get; set; }
    public int PresentCount { get; set; }
    public int LateCount { get; set; }
    public int AbsentCount { get; set; }
}

public class CourseRateDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public decimal AttendancePercentage { get; set; }
}

public class AtRiskStudentDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public decimal AttendancePercentage { get; set; }
}

public class ReportAnalyticsDto
{
    public StatusDistributionDto StatusDistribution { get; set; } = new();
    public IReadOnlyList<HistoricalMonthlyRateDto> HistoricalMonthlyRates { get; set; } = [];
    public IReadOnlyList<ClassComparisonDto> ClassComparisons { get; set; } = [];
    public ReportAnalyticsKpisDto Kpis { get; set; } = new();
}

public class StatusDistributionDto
{
    public int Present { get; set; }
    public int Late { get; set; }
    public int Absent { get; set; }
    public int Excused { get; set; }
}

public class HistoricalMonthlyRateDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal AttendancePercentage { get; set; }
}

public class ClassComparisonDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalEnrolled { get; set; }
    public int TotalEvents { get; set; }
    public decimal AverageRate { get; set; }
}

public class ReportAnalyticsKpisDto
{
    public int TotalAttendanceEvents { get; set; }
    public int EnrolledLearners { get; set; }
    public decimal GlobalAverageRate { get; set; }
    public int LowAttendanceCount { get; set; }
}
