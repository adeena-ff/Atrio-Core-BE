using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atrio.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DbInitializer));
        await db.Database.MigrateAsync(cancellationToken);

        var resetRequested = bool.TryParse(configuration["DemoData:Reset"], out var reset) && reset;
        var summary = await DemoDataSeeder.SeedAsync(db, resetRequested, cancellationToken);
        logger.LogInformation(
            "Demo data {SeedAction}: Users={Users}; Classes={Classes}; Students={Students}; AttendanceRecords={AttendanceRecords}.",
            summary.ResetApplied ? "reset" : "verified",
            summary.Users,
            summary.Classes,
            summary.Students,
            summary.AttendanceRecords);
    }
}
