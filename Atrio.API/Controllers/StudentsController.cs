using Atrio.API.DTOs;
using Atrio.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StudentDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await studentService.GetAllAsync(cancellationToken));
    }
}
