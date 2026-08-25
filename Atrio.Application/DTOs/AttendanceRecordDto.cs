using System.ComponentModel.DataAnnotations;
using Atrio.Domain.Enums;

namespace Atrio.Application.DTOs;

public class AttendanceRecordDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public Guid RecordedByUserId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class UpsertAttendanceDto
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid ClassId { get; set; }

    [Required]
    public DateOnly AttendanceDate { get; set; }

    [Required]
    public AttendanceStatus Status { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public Guid? RecordedByUserId { get; set; }
}

public class AttendanceHistoryDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal AttendancePercentage { get; set; }
    public IReadOnlyList<AttendanceRecordDto> Records { get; set; } = [];
}
