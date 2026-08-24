using Atrio.API.Data;
using Atrio.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Atrio.API.Services;

public class AuthService(ApplicationDbContext dbContext) : IAuthService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new LoginResponseDto
        {
            Token = string.Empty,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            }
        };
    }
}
