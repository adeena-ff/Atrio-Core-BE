using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("api/attendance/reports")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet("monthly")]
    public async Task<ActionResult<MonthlyReportDto>> Monthly(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] Guid? classId,
        [FromQuery] string? search,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(await reportService.GetMonthlyAsync(new MonthlyReportQuery
        {
            Year = year == 0 ? DateTime.UtcNow.Year : year,
            Month = month == 0 ? DateTime.UtcNow.Month : month,
            ClassId = classId,
            Search = search,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TeacherId = GetTeacherIdOrNull()
        }, cancellationToken));
    }

    private Guid? GetTeacherIdOrNull() =>
        User.IsInRole("Teacher") && Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var userId)
            ? userId
            : null;
}
