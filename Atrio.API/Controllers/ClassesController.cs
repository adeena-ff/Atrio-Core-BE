using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassesController(IClassService classService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClassDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await classService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ClassDto>> Create([FromBody] CreateClassDto dto, CancellationToken cancellationToken)
    {
        return Ok(await classService.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClassDto>> Update(Guid id, [FromBody] UpdateClassDto dto, CancellationToken cancellationToken)
    {
        return Ok(await classService.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignStudentDto dto, CancellationToken cancellationToken)
    {
        await classService.AssignStudentAsync(dto, cancellationToken);
        return NoContent();
    }
}
