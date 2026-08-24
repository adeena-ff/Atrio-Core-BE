using Atrio.API.DTOs;

namespace Atrio.API.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
