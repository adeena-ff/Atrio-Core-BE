using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController(IAttendanceService attendanceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AttendanceRecordDto>>> GetAll(
        [FromQuery] Guid? classId,
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        if (classId.HasValue && date.HasValue)
        {
            return Ok(await attendanceService.GetByClassAndDateAsync(classId.Value, date.Value, cancellationToken));
        }

        return Ok(await attendanceService.GetAllAsync(cancellationToken));
    }

    [HttpGet("rollcall")]
    public async Task<ActionResult<IReadOnlyList<AttendanceRecordDto>>> Rollcall(
        [FromQuery] Guid classId,
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        return Ok(await attendanceService.GetByClassAndDateAsync(classId, date, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<AttendanceRecordDto>> Upsert(
        [FromBody] UpsertAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await attendanceService.UpsertAsync(dto, cancellationToken));
    }

    [HttpPost("mark")]
    public async Task<ActionResult<AttendanceRecordDto>> Mark(
        [FromBody] UpsertAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await attendanceService.UpsertAsync(dto, cancellationToken));
    }
}
