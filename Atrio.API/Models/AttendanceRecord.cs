namespace Atrio.API.Models;

public class AttendanceRecord
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public Guid RecordedByUserId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Student Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public User RecordedByUser { get; set; } = null!;
}
