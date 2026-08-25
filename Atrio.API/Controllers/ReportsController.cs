using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("api/attendance/reports")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("monthly")]
    public async Task<ActionResult<MonthlyReportDto>> Monthly(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] Guid? classId,
        CancellationToken cancellationToken)
    {
        return Ok(await reportService.GetMonthlyAsync(new MonthlyReportQuery
        {
            Year = year == 0 ? DateTime.UtcNow.Year : year,
            Month = month == 0 ? DateTime.UtcNow.Month : month,
            ClassId = classId
        }, cancellationToken));
    }
}
