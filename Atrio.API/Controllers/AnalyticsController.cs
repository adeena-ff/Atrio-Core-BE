using System.Security.Claims;
using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize(Roles = "Admin,Teacher")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardAnalyticsDto>> Dashboard(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherScope(out var teacherId)) return Forbid();
        return Ok(await analyticsService.GetDashboardAsync(teacherId, cancellationToken));
    }

    [HttpGet("reports")]
    public async Task<ActionResult<ReportAnalyticsDto>> Reports([FromQuery] Guid? classId, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherScope(out var teacherId)) return Forbid();
        if (startDate.HasValue && endDate.HasValue && startDate > endDate) return BadRequest(new { message = "startDate must not be later than endDate." });
        return Ok(await analyticsService.GetReportsAsync(teacherId, classId, startDate, endDate, cancellationToken));
    }

    private bool TryGetTeacherScope(out Guid? teacherId)
    {
        teacherId = null;
        if (!User.IsInRole("Teacher")) return true;
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(id, out var userId)) return false;
        teacherId = userId;
        return true;
    }
}
