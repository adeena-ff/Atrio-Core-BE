using System.ComponentModel.DataAnnotations;

namespace Atrio.Domain.Enums;

public enum UserRole
{
    [Display(Name = "Admin")]
    Admin = 1,

    [Display(Name = "Teacher")]
    Teacher = 2
}
