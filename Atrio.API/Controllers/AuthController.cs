using Atrio.API.DTOs;
using Atrio.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atrio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }
}
