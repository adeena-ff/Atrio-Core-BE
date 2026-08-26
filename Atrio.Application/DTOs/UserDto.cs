using Atrio.Domain.Enums;

namespace Atrio.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public IReadOnlyList<Guid> AssignedClassIds { get; set; } = [];
}
