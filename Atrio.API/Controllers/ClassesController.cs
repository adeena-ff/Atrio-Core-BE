using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassesController(IClassService classService) : ControllerBase
{
    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClassDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await classService.GetAllAsync(GetTeacherIdOrNull(), cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ClassDto>> Create([FromBody] CreateClassDto dto, CancellationToken cancellationToken)
    {
        return Ok(await classService.CreateAsync(dto, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClassDto>> Update(Guid id, [FromBody] UpdateClassDto dto, CancellationToken cancellationToken)
    {
        return Ok(await classService.UpdateAsync(id, dto, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignStudentDto dto, CancellationToken cancellationToken)
    {
        await classService.AssignStudentAsync(dto, cancellationToken);
        return NoContent();
    }

    private Guid? GetTeacherIdOrNull() => User.IsInRole("Teacher") && Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var userId) ? userId : null;
}
