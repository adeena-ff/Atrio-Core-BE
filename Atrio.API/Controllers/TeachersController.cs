using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TeachersController(ITeacherService teacherService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Atrio.Application.Common.Models.PagedResponse<TeacherDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? classId,
        [FromQuery] string? status,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10) =>
        Ok(await teacherService.GetAllAsync(new TeacherSearchQuery
        {
            Search = search,
            ClassId = classId,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        }, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<TeacherDto>> Create([FromBody] CreateTeacherDto dto, CancellationToken cancellationToken)
    {
        var teacher = await teacherService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = teacher.Id }, teacher);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TeacherDto>> Update(Guid id, [FromBody] UpdateTeacherDto dto, CancellationToken cancellationToken) => Ok(await teacherService.UpdateAsync(id, dto, cancellationToken));

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken) { await teacherService.DeactivateAsync(id, cancellationToken); return NoContent(); }
}
