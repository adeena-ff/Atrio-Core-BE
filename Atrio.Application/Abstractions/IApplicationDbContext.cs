using Atrio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Student> Students { get; }
    DbSet<Class> Classes { get; }
    DbSet<AttendanceRecord> AttendanceRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
