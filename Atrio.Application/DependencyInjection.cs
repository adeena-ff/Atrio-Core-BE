using Atrio.Application.Interfaces;
using Atrio.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Atrio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IReportService, ReportService>();
        return services;
    }
}
