using Atrio.Application.DTOs;
using Atrio.Domain.Entities;

namespace Atrio.Application.Mapping;

public static class DtoMapper
{
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role,
        IsActive = user.IsActive
    };

    public static StudentDto ToDto(this Student student, decimal attendancePercentage = 0) => new()
    {
        Id = student.Id,
        FirstName = student.FirstName,
        LastName = student.LastName,
        Email = student.Email,
        EnrollmentNumber = student.EnrollmentNumber,
        ClassId = student.ClassId,
        ClassName = student.Class?.Name,
        IsActive = student.IsActive,
        AttendancePercentage = attendancePercentage
    };

    public static ClassDto ToDto(this Class classEntity, int studentCount = 0) => new()
    {
        Id = classEntity.Id,
        Name = classEntity.Name,
        Code = classEntity.Code,
        AcademicYear = classEntity.AcademicYear,
        IsActive = classEntity.IsActive,
        StudentCount = studentCount
    };

    public static AttendanceRecordDto ToDto(this AttendanceRecord record) => new()
    {
        Id = record.Id,
        StudentId = record.StudentId,
        StudentName = record.Student is null
            ? string.Empty
            : $"{record.Student.FirstName} {record.Student.LastName}",
        ClassId = record.ClassId,
        RecordedByUserId = record.RecordedByUserId,
        AttendanceDate = record.AttendanceDate,
        Status = record.Status,
        Notes = record.Notes
    };
}
