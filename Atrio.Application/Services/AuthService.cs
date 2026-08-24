using Atrio.Application.Abstractions;
using Atrio.Application.Common;
using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Atrio.Application.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Application.Services;

public class AuthService(IApplicationDbContext dbContext) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw AppValidationException.Single(nameof(request.Email), "Email and password are required.");
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLower() && u.IsActive, cancellationToken);

        if (user is null)
        {
            throw AppValidationException.Single(nameof(request.Email), "Invalid credentials.");
        }

        return new LoginResponseDto
        {
            Token = $"atrio-dev-{user.Id}",
            User = user.ToDto()
        };
    }
}
