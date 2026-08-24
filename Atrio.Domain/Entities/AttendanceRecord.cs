using System.ComponentModel.DataAnnotations;
using Atrio.Domain.Enums;

namespace Atrio.Domain.Entities;

public class AttendanceRecord
{
    public Guid Id { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid ClassId { get; set; }

    [Required]
    public Guid RecordedByUserId { get; set; }

    [Required]
    public DateOnly AttendanceDate { get; set; }

    [Required]
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public Student Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public User RecordedByUser { get; set; } = null!;
}
