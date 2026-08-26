using Atrio.Application.Abstractions;
using Atrio.Application.DTOs;
using Atrio.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IApplicationDbContext db) : ControllerBase
{
    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var records = await db.AttendanceRecords.AsNoTracking().Where(x => x.AttendanceDate == today).ToListAsync(cancellationToken);
        var eligible = records.Count(x => x.Status != AttendanceStatus.Excused);
        var students = await db.Students.AsNoTracking().Include(x => x.AttendanceRecords).Where(x => x.IsActive).ToListAsync(cancellationToken);
        var lowAttendanceAlerts = students.Count(student =>
        {
            var conducted = student.AttendanceRecords.Count(record => record.Status != AttendanceStatus.Excused);
            if (conducted == 0) return false;
            var attended = student.AttendanceRecords.Count(record => record.Status is AttendanceStatus.Present or AttendanceStatus.Late);
            return (decimal)attended / conducted * 100 < 75;
        });
        return Ok(new DashboardDto { TotalStudents = students.Count, ActiveClasses = await db.Classes.CountAsync(x => x.IsActive, cancellationToken), TodayAttendancePercentage = eligible == 0 ? 0 : Math.Round((decimal)records.Count(x => x.Status is AttendanceStatus.Present or AttendanceStatus.Late) / eligible * 100, 2), LowAttendanceAlerts = lowAttendanceAlerts });
    }
}
