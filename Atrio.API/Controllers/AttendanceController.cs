using Atrio.API.DTOs;
using Atrio.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController(IAttendanceService attendanceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AttendanceRecordDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await attendanceService.GetAllAsync(cancellationToken));
    }
}
