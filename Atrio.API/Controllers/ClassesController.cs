using Atrio.API.DTOs;
using Atrio.API.Services;
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
}
