using Atrio.Application.Abstractions;
using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Atrio.Domain.Enums;
using Atrio.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Application.Services;

public class ReportService(IApplicationDbContext dbContext) : IReportService
{
    public async Task<MonthlyReportDto> GetMonthlyAsync(MonthlyReportQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Month is < 1 or > 12)
        {
            query.Month = DateTime.UtcNow.Month;
        }

        var start = new DateOnly(query.Year, query.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var studentsQuery = dbContext.Students
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.AttendanceRecords.Where(r => r.AttendanceDate >= start && r.AttendanceDate <= end))
            .Where(s => s.IsActive);

        if (query.ClassId.HasValue)
        {
            studentsQuery = studentsQuery.Where(s => s.ClassId == query.ClassId.Value);
        }

        var students = await studentsQuery
            .OrderBy(s => s.LastName)
            .ToListAsync(cancellationToken);

        var rows = students.Select(student =>
        {
            var present = student.AttendanceRecords.Count(r => r.Status == AttendanceStatus.Present);
            var late = student.AttendanceRecords.Count(r => r.Status == AttendanceStatus.Late);
            var absent = student.AttendanceRecords.Count(r => r.Status == AttendanceStatus.Absent);
            var excused = student.AttendanceRecords.Count(r => r.Status == AttendanceStatus.Excused);

            return new StudentMonthlyRowDto
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                EnrollmentNumber = student.EnrollmentNumber,
                Present = present,
                Absent = absent,
                Late = late,
                Excused = excused,
                Percentage = AttendanceCalculator.CalculatePercentage(present, late, absent, excused)
            };
        }).ToList();

        var overall = rows.Count == 0 ? 0m : Math.Round(rows.Average(r => r.Percentage), 2);
        var className = query.ClassId.HasValue
            ? students.FirstOrDefault()?.Class?.Name
            : "All classes";

        return new MonthlyReportDto
        {
            Year = query.Year,
            Month = query.Month,
            ClassName = className,
            OverallPercentage = overall,
            Students = rows
        };
    }
}
