using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Atrio.Application.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace Atrio.API.Services;

public interface IJwtTokenService { string Create(UserDto user); }

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string Create(UserDto user)
    {
        var jwt = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(JwtRegisteredClaimNames.Email, user.Email), new Claim(ClaimTypes.Name, user.FullName), new Claim(ClaimTypes.Role, user.Role.ToString()) };
        var expiry = DateTime.UtcNow.AddMinutes(int.TryParse(jwt["ExpiryMinutes"], out var minutes) ? minutes : 480);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims, expires: expiry, signingCredentials: credentials));
    }
}
