using Atrio.Application.Abstractions;
using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Atrio.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Application.Services;

public class AnalyticsService(IApplicationDbContext dbContext) : IAnalyticsService
{
    public async Task<DashboardAnalyticsDto> GetDashboardAsync(Guid? teacherId, CancellationToken cancellationToken = default)
    {
        var classes = await ScopedClasses(teacherId).ToListAsync(cancellationToken);
        var classIds = classes.Select(course => course.Id).ToArray();
        var students = await dbContext.Students.AsNoTracking().Where(student => student.IsActive && classIds.Contains(student.ClassId))
            .Select(student => new { student.Id, student.FirstName, student.LastName, student.EnrollmentNumber, student.ClassId })
            .ToListAsync(cancellationToken);
        var aggregate = await AggregateAttendance(classIds, null, null, cancellationToken);
        var aggregateByStudent = aggregate.GroupBy(row => row.StudentId).ToDictionary(group => group.Key, group => group.Single());
        var classNames = classes.ToDictionary(course => course.Id, course => course.Name);
        var totalEligible = aggregate.Sum(row => row.Present + row.Late + row.Absent);
        var totalAttended = aggregate.Sum(row => row.Present + row.Late);
        var atRiskStudents = students.Select(student => new
            {
                Student = student,
                Stats = aggregateByStudent.GetValueOrDefault(student.Id)
            })
            .Where(item => item.Stats is not null && Rate(item.Stats.Present, item.Stats.Late, item.Stats.Absent) < 75)
            .Select(item => new AtRiskStudentDto
            {
                StudentId = item.Student.Id,
                StudentName = $"{item.Student.FirstName} {item.Student.LastName}",
                EnrollmentNumber = item.Student.EnrollmentNumber,
                ClassName = classNames.GetValueOrDefault(item.Student.ClassId, string.Empty),
                AttendancePercentage = Rate(item.Stats!.Present, item.Stats.Late, item.Stats.Absent),
                ClassId = item.Student.ClassId,
                Percentage = Rate(item.Stats.Present, item.Stats.Late, item.Stats.Absent)
            })
            .OrderBy(item => item.AttendancePercentage)
            .ToList();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var trendRows = await dbContext.AttendanceRecords.AsNoTracking()
            .Where(record => classIds.Contains(record.ClassId) && record.AttendanceDate >= today.AddDays(-29) && record.AttendanceDate <= today)
            .GroupBy(record => record.AttendanceDate)
            .Select(group => new DailyTrendDto
            {
                Date = group.Key,
                PresentCount = group.Count(record => record.Status == AttendanceStatus.Present),
                LateCount = group.Count(record => record.Status == AttendanceStatus.Late),
                AbsentCount = group.Count(record => record.Status == AttendanceStatus.Absent)
            })
            .OrderBy(item => item.Date)
            .ToListAsync(cancellationToken);

        var todayStats = await dbContext.AttendanceRecords.AsNoTracking()
            .Where(record => classIds.Contains(record.ClassId) && record.AttendanceDate == today)
            .GroupBy(record => record.ClassId)
            .Select(group => new { ClassId = group.Key, Marked = group.Count(), Present = group.Count(record => record.Status == AttendanceStatus.Present), Late = group.Count(record => record.Status == AttendanceStatus.Late), Absent = group.Count(record => record.Status == AttendanceStatus.Absent) })
            .ToListAsync(cancellationToken);
        var todayByClass = todayStats.ToDictionary(item => item.ClassId);

        var statsByClass = aggregate.GroupBy(row => row.ClassId).ToDictionary(group => group.Key, group => new { Present = group.Sum(row => row.Present), Late = group.Sum(row => row.Late), Absent = group.Sum(row => row.Absent) });
        return new DashboardAnalyticsDto
        {
            Role = teacherId.HasValue ? UserRole.Teacher : UserRole.Admin,
            Metrics = new DashboardMetricsDto
            {
                TotalStudents = students.Count, ActiveClasses = classes.Count,
                TodayAttendancePercentage = Rate(todayStats.Sum(item => item.Present), todayStats.Sum(item => item.Late), todayStats.Sum(item => item.Absent)),
                AtRiskCount = atRiskStudents.Count
            },
            DailyTrend = trendRows.Select(row => new DailyAttendancePointDto { Date = row.Date, Present = row.PresentCount, Late = row.LateCount, Absent = row.AbsentCount, Excused = 0, Percentage = Rate(row.PresentCount, row.LateCount, row.AbsentCount) }).ToList(),
            CourseBreakdown = classes.Select(course =>
            {
                var stats = statsByClass.GetValueOrDefault(course.Id);
                return new NamedMetricDto { Id = course.Id.ToString(), Name = course.Name, Value = stats is null ? 0 : Rate(stats.Present, stats.Late, stats.Absent), SecondaryValue = students.Count(student => student.ClassId == course.Id) };
            }).OrderBy(item => item.Name).ToList(),
            MyClassesToday = teacherId.HasValue ? classes.Select(course =>
            {
                var stats = todayByClass.GetValueOrDefault(course.Id);
                return new ClassTodayGaugeDto { ClassId = course.Id, ClassName = course.Name, Code = course.Code, StudentCount = students.Count(student => student.ClassId == course.Id), MarkedCount = stats?.Marked ?? 0, Percentage = stats is null ? 0 : Rate(stats.Present, stats.Late, stats.Absent) };
            }).ToList() : [],
            Kpis = new DashboardAnalyticsKpisDto { TotalStudents = students.Count, TotalClasses = classes.Count, AverageAttendanceRate = Rate(totalAttended, 0, totalEligible - totalAttended), AtRiskCount = atRiskStudents.Count },
            DailyTrends = trendRows,
            CourseRates = classes.Select(course =>
            {
                var stats = statsByClass.GetValueOrDefault(course.Id);
                return new CourseRateDto { ClassId = course.Id, ClassName = course.Name, AttendancePercentage = stats is null ? 0 : Rate(stats.Present, stats.Late, stats.Absent) };
            }).OrderBy(course => course.ClassName).ToList(),
            AtRiskStudents = atRiskStudents
        };
    }

    public async Task<ReportAnalyticsDto> GetReportsAsync(Guid? teacherId, Guid? classId, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken = default)
    {
        var classesQuery = ScopedClasses(teacherId);
        if (classId.HasValue) classesQuery = classesQuery.Where(course => course.Id == classId.Value);
        var classes = await classesQuery.ToListAsync(cancellationToken);
        var classIds = classes.Select(course => course.Id).ToArray();
        var end = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var start = startDate ?? end.AddDays(-60);
        if (start > end) throw new ArgumentException("Start date cannot be later than end date.");

        var selectedAggregate = await AggregateAttendance(classIds, start, end, cancellationToken);
        var presentCount = selectedAggregate.Sum(row => row.Present);
        var lateCount = selectedAggregate.Sum(row => row.Late);
        var absentCount = selectedAggregate.Sum(row => row.Absent);
        var excusedCount = selectedAggregate.Sum(row => row.Excused);
        var students = await dbContext.Students.AsNoTracking().Where(student => student.IsActive && classIds.Contains(student.ClassId))
            .Select(student => new { student.Id, student.ClassId, student.FirstName, student.LastName, student.EnrollmentNumber }).ToListAsync(cancellationToken);
        var statsByStudent = selectedAggregate.GroupBy(row => row.StudentId).ToDictionary(group => group.Key, group => group.Single());
        var historicalRates = await dbContext.AttendanceRecords.AsNoTracking()
            .Where(record => classIds.Contains(record.ClassId))
            .GroupBy(record => new { record.AttendanceDate.Year, record.AttendanceDate.Month })
            .Select(group => new { group.Key.Year, group.Key.Month, Present = group.Count(record => record.Status == AttendanceStatus.Present), Late = group.Count(record => record.Status == AttendanceStatus.Late), Absent = group.Count(record => record.Status == AttendanceStatus.Absent) })
            .OrderBy(group => group.Year).ThenBy(group => group.Month)
            .ToListAsync(cancellationToken);
        var statsByClass = selectedAggregate.GroupBy(row => row.ClassId).ToDictionary(group => group.Key, group => group.ToList());

        var classNames = classes.ToDictionary(course => course.Id, course => course.Name);
        var selectedRecords = await dbContext.AttendanceRecords.AsNoTracking()
            .Where(record => classIds.Contains(record.ClassId) && record.AttendanceDate >= start && record.AttendanceDate <= end)
            .Select(record => new { record.StudentId, record.ClassId, record.AttendanceDate, record.Status })
            .OrderByDescending(record => record.AttendanceDate)
            .ToListAsync(cancellationToken);
        var classComparisons = classes.Select(course =>
        {
            var stats = statsByClass.GetValueOrDefault(course.Id) ?? [];
            var present = stats.Sum(row => row.Present); var late = stats.Sum(row => row.Late); var absent = stats.Sum(row => row.Absent);
            return new ClassComparisonDto { ClassId = course.Id, ClassName = course.Name, TotalEnrolled = students.Count(student => student.ClassId == course.Id), TotalEvents = stats.Sum(row => row.Present + row.Late + row.Absent + row.Excused), AverageRate = Rate(present, late, absent) };
        }).OrderBy(comparison => comparison.ClassName).ToList();
        var classBreakdown = classComparisons.Select(comparison =>
        {
            var stats = statsByClass.GetValueOrDefault(comparison.ClassId) ?? [];
            var present = stats.Sum(row => row.Present); var late = stats.Sum(row => row.Late); var absent = stats.Sum(row => row.Absent); var excused = stats.Sum(row => row.Excused);
            return new ClassBreakdownDto
            {
                ClassId = comparison.ClassId, ClassName = comparison.ClassName, Code = classes.Single(course => course.Id == comparison.ClassId).Code,
                Percentage = Rate(present, late, absent), Present = present, Late = late, Absent = absent, Excused = excused,
                Students = students.Where(student => student.ClassId == comparison.ClassId).Select(student =>
                {
                    var studentStats = statsByStudent.GetValueOrDefault(student.Id);
                    return new StudentMonthlyRowDto { StudentId = student.Id, StudentName = $"{student.FirstName} {student.LastName}", EnrollmentNumber = student.EnrollmentNumber, Present = studentStats?.Present ?? 0, Late = studentStats?.Late ?? 0, Absent = studentStats?.Absent ?? 0, Excused = studentStats?.Excused ?? 0, Percentage = studentStats is null ? 0 : Rate(studentStats.Present, studentStats.Late, studentStats.Absent) };
                }).OrderBy(student => student.StudentName).ToList()
            };
        }).ToList();

        return new ReportAnalyticsDto
        {
            Role = teacherId.HasValue ? UserRole.Teacher : UserRole.Admin,
            StartDate = start, EndDate = end, ClassId = classId,
            StatusDistribution = [new() { Id = "present", Name = "Present", Value = presentCount }, new() { Id = "late", Name = "Late", Value = lateCount }, new() { Id = "absent", Name = "Absent", Value = absentCount }, new() { Id = "excused", Name = "Excused", Value = excusedCount }],
            CoursePerformance = classComparisons.Select(item => new NamedMetricDto { Id = item.ClassId.ToString(), Name = item.ClassName, Value = item.AverageRate, SecondaryValue = item.TotalEnrolled }).ToList(),
            CourseRadar = classComparisons.Select(item => new NamedMetricDto { Id = item.ClassId.ToString(), Name = item.ClassName, Value = item.AverageRate, SecondaryValue = item.TotalEvents }).ToList(),
            ClassBreakdown = classBreakdown,
            StudentHistories = students.Select(student =>
            {
                var stats = statsByStudent.GetValueOrDefault(student.Id);
                return new StudentHistoryAnalyticsDto { StudentId = student.Id, StudentName = $"{student.FirstName} {student.LastName}", EnrollmentNumber = student.EnrollmentNumber, Percentage = stats is null ? 0 : Rate(stats.Present, stats.Late, stats.Absent), Timeline = selectedRecords.Where(record => record.StudentId == student.Id).Take(12).Select(record => new StudentHistoryPointDto { Date = record.AttendanceDate, Status = record.Status, ClassName = classNames.GetValueOrDefault(record.ClassId, string.Empty) }).ToList() };
            }).OrderBy(student => student.Percentage).ToList(),
            HistoricalMonthlyRates = historicalRates.Select(rate => new HistoricalMonthlyRateDto { Year = rate.Year, Month = rate.Month, AttendancePercentage = Rate(rate.Present, rate.Late, rate.Absent) }).ToList(),
            ClassComparisons = classComparisons,
            Kpis = new ReportAnalyticsKpisDto
            {
                TotalAttendanceEvents = presentCount + lateCount + absentCount + excusedCount,
                EnrolledLearners = students.Count,
                GlobalAverageRate = Rate(presentCount, lateCount, absentCount),
                LowAttendanceCount = students.Count(student => statsByStudent.TryGetValue(student.Id, out var stats) && Rate(stats.Present, stats.Late, stats.Absent) < 75),
                TotalEvents = presentCount + lateCount + absentCount + excusedCount,
                ActiveLearners = students.Count,
                SystemAveragePercentage = Rate(presentCount, lateCount, absentCount),
                AtRiskCount = students.Count(student => statsByStudent.TryGetValue(student.Id, out var stats) && Rate(stats.Present, stats.Late, stats.Absent) < 75)
            }
        };
    }

    private IQueryable<Domain.Entities.Class> ScopedClasses(Guid? teacherId)
    {
        var query = dbContext.Classes.AsNoTracking().Where(course => course.IsActive);
        return teacherId.HasValue ? query.Where(course => course.TeacherId == teacherId.Value) : query;
    }

    private Task<List<AttendanceAggregate>> AggregateAttendance(Guid[] classIds, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken)
    {
        var query = dbContext.AttendanceRecords.AsNoTracking().Where(record => classIds.Contains(record.ClassId));
        if (startDate.HasValue) query = query.Where(record => record.AttendanceDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(record => record.AttendanceDate <= endDate.Value);
        return query.GroupBy(record => new { record.ClassId, record.StudentId })
            .Select(group => new AttendanceAggregate
            {
                ClassId = group.Key.ClassId, StudentId = group.Key.StudentId,
                Present = group.Count(record => record.Status == AttendanceStatus.Present), Late = group.Count(record => record.Status == AttendanceStatus.Late),
                Absent = group.Count(record => record.Status == AttendanceStatus.Absent), Excused = group.Count(record => record.Status == AttendanceStatus.Excused)
            }).ToListAsync(cancellationToken);
    }

    private static decimal Rate(int present, int late, int absent)
    {
        var eligible = present + late + absent;
        return eligible == 0 ? 0 : Math.Round((present + late) * 100m / eligible, 2);
    }

    private sealed class AttendanceAggregate
    {
        public Guid ClassId { get; init; }
        public Guid StudentId { get; init; }
        public int Present { get; init; }
        public int Late { get; init; }
        public int Absent { get; init; }
        public int Excused { get; init; }
    }
}
