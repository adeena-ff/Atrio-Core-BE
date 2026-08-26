using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet]
    public async Task<ActionResult<Atrio.Application.Common.Models.PagedResponse<StudentDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] Guid? classId,
        [FromQuery] string? status,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(await studentService.SearchAsync(new StudentSearchQuery
        {
            Search = search,
            ClassId = classId,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TeacherId = GetTeacherIdOrNull()
        }, cancellationToken));
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var student = await studentService.GetByIdAsync(id, cancellationToken);
        return student is null ? NotFound() : Ok(student);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<AttendanceHistoryDto>> History(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await studentService.GetHistoryAsync(id, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto dto, CancellationToken cancellationToken)
    {
        var created = await studentService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StudentDto>> Update(Guid id, [FromBody] UpdateStudentDto dto, CancellationToken cancellationToken)
    {
        return Ok(await studentService.UpdateAsync(id, dto, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await studentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private Guid? GetTeacherIdOrNull() =>
        User.IsInRole("Teacher") && Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var userId)
            ? userId
            : null;
}
