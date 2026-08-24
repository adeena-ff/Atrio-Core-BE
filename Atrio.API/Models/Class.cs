namespace Atrio.API.Models;

public class Class
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Student> Students { get; set; } = [];
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = [];
}
