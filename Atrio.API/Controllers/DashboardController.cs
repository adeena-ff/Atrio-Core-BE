using Atrio.Application.Abstractions;
using Atrio.Application.DTOs;
using Atrio.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var records = await db.AttendanceRecords.AsNoTracking().Where(x => x.AttendanceDate == today).ToListAsync(cancellationToken);
        var eligible = records.Count(x => x.Status != AttendanceStatus.Excused);
        return Ok(new DashboardDto { TotalStudents = await db.Students.CountAsync(x => x.IsActive, cancellationToken), ActiveClasses = await db.Classes.CountAsync(x => x.IsActive, cancellationToken), TodayAttendancePercentage = eligible == 0 ? 0 : Math.Round((decimal)records.Count(x => x.Status is AttendanceStatus.Present or AttendanceStatus.Late) / eligible * 100, 2), LowAttendanceAlerts = 0 });
    }
}
