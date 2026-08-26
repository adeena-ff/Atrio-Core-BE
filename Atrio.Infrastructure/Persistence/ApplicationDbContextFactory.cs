using Atrio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Atrio.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var apiDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Atrio.API");
        if (!Directory.Exists(apiDirectory)) apiDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "Atrio.API");
        apiDirectory = Path.GetFullPath(apiDirectory);
        DotEnv.Load(Path.Combine(apiDirectory, ".env"));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=AtrioDb;Username=postgres;Password=your_password";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}
