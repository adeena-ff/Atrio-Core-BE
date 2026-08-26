using System.ComponentModel.DataAnnotations;

namespace Atrio.Domain.Entities;

public class Class
{
    public Guid Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string AcademicYear { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? TeacherId { get; set; }
    public User? Teacher { get; set; }

    public ICollection<Student> Students { get; set; } = [];
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = [];
}
