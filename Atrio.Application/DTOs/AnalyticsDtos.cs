using Atrio.Domain.Enums;

namespace Atrio.Application.DTOs;

public class DashboardAnalyticsDto
{
    public UserRole Role { get; set; }
    public DashboardMetricsDto Metrics { get; set; } = new();
    public IReadOnlyList<DailyAttendancePointDto> DailyTrend { get; set; } = [];
    public IReadOnlyList<NamedMetricDto> CourseBreakdown { get; set; } = [];
    public IReadOnlyList<ClassTodayGaugeDto> MyClassesToday { get; set; } = [];
    public DashboardAnalyticsKpisDto Kpis { get; set; } = new();
    public IReadOnlyList<DailyTrendDto> DailyTrends { get; set; } = [];
    public IReadOnlyList<CourseRateDto> CourseRates { get; set; } = [];
    public IReadOnlyList<AtRiskStudentDto> AtRiskStudents { get; set; } = [];
}

public class DashboardMetricsDto
{
    public int TotalStudents { get; set; }
    public int ActiveClasses { get; set; }
    public decimal TodayAttendancePercentage { get; set; }
    public int AtRiskCount { get; set; }
}

public class DailyAttendancePointDto
{
    public DateOnly Date { get; set; }
    public decimal Percentage { get; set; }
    public int Present { get; set; }
    public int Late { get; set; }
    public int Absent { get; set; }
    public int Excused { get; set; }
}

public class NamedMetricDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal? SecondaryValue { get; set; }
}

public class ClassTodayGaugeDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int MarkedCount { get; set; }
    public int StudentCount { get; set; }
    public decimal Percentage { get; set; }
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
    public Guid ClassId { get; set; }
    public decimal Percentage { get; set; }
}

public class ReportAnalyticsDto
{
    public UserRole Role { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid? ClassId { get; set; }
    public IReadOnlyList<NamedMetricDto> StatusDistribution { get; set; } = [];
    public IReadOnlyList<NamedMetricDto> CoursePerformance { get; set; } = [];
    public IReadOnlyList<NamedMetricDto> CourseRadar { get; set; } = [];
    public IReadOnlyList<ClassBreakdownDto> ClassBreakdown { get; set; } = [];
    public IReadOnlyList<StudentHistoryAnalyticsDto> StudentHistories { get; set; } = [];
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
    public int TotalEvents { get; set; }
    public int ActiveLearners { get; set; }
    public decimal SystemAveragePercentage { get; set; }
    public int AtRiskCount { get; set; }
}

public class ClassBreakdownDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public int Present { get; set; }
    public int Late { get; set; }
    public int Absent { get; set; }
    public int Excused { get; set; }
    public IReadOnlyList<StudentMonthlyRowDto> Students { get; set; } = [];
}

public class StudentHistoryAnalyticsDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public IReadOnlyList<StudentHistoryPointDto> Timeline { get; set; } = [];
}

public class StudentHistoryPointDto
{
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string ClassName { get; set; } = string.Empty;
}
