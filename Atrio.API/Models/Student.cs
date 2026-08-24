namespace Atrio.API.Models;

public class Student
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Class Class { get; set; } = null!;
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = [];
}
