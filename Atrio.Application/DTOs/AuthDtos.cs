using System.ComponentModel.DataAnnotations;

namespace Atrio.Application.DTOs;

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

public class DashboardDto
{
    public int TotalStudents { get; set; }
    public int ActiveClasses { get; set; }
    public decimal TodayAttendancePercentage { get; set; }
    public int LowAttendanceAlerts { get; set; }
}
