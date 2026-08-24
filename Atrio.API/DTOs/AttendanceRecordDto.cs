using Atrio.API.Models;

namespace Atrio.API.DTOs;

public class AttendanceRecordDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public Guid RecordedByUserId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateAttendanceRecordDto
{
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
