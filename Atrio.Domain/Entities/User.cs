using System.ComponentModel.DataAnnotations;
using Atrio.Domain.Enums;

namespace Atrio.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Admin;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<AttendanceRecord> RecordedAttendance { get; set; } = [];
}
