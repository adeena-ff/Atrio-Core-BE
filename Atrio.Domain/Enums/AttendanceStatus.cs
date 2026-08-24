using System.ComponentModel.DataAnnotations;

namespace Atrio.Domain.Enums;

public enum AttendanceStatus
{
    [Display(Name = "Present")]
    Present = 1,

    [Display(Name = "Absent")]
    Absent = 2,

    [Display(Name = "Late")]
    Late = 3,

    [Display(Name = "Excused")]
    Excused = 4
}
