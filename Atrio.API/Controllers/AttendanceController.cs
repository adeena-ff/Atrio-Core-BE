using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Atrio.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController(IAttendanceService attendanceService, IApplicationDbContext db) : ControllerBase
{
    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AttendanceRecordDto>>> GetAll(
        [FromQuery] Guid? classId,
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        if (classId.HasValue && date.HasValue)
        {
            return Ok(await attendanceService.GetByClassAndDateAsync(classId.Value, date.Value, GetTeacherIdOrNull(), cancellationToken));
        }

        return Ok(await attendanceService.GetAllAsync(GetTeacherIdOrNull(), cancellationToken));
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet("rollcall")]
    public async Task<ActionResult<IReadOnlyList<AttendanceRecordDto>>> Rollcall(
        [FromQuery] Guid classId,
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        return Ok(await attendanceService.GetByClassAndDateAsync(classId, date, GetTeacherIdOrNull(), cancellationToken));
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPut]
    public async Task<ActionResult<AttendanceRecordDto>> Upsert(
        [FromBody] UpsertAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        if (!await IsWithinTeacherClassScopeAsync(dto.ClassId, cancellationToken)) return Forbid();
        dto.RecordedByUserId = GetCurrentUserId();
        return Ok(await attendanceService.UpsertAsync(dto, cancellationToken));
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost("mark")]
    public async Task<ActionResult<AttendanceRecordDto>> Mark(
        [FromBody] UpsertAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        if (!await IsWithinTeacherClassScopeAsync(dto.ClassId, cancellationToken)) return Forbid();
        dto.RecordedByUserId = GetCurrentUserId();
        return Ok(await attendanceService.UpsertAsync(dto, cancellationToken));
    }

    private Guid? GetTeacherIdOrNull() => User.IsInRole("Teacher") ? GetCurrentUserId() : null;
    private Guid? GetCurrentUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var userId) ? userId : null;
    private async Task<bool> IsWithinTeacherClassScopeAsync(Guid classId, CancellationToken cancellationToken)
    {
        var teacherId = GetTeacherIdOrNull();
        return !teacherId.HasValue || await db.Classes.AnyAsync(classEntity => classEntity.Id == classId && classEntity.TeacherId == teacherId.Value, cancellationToken);
    }
}
