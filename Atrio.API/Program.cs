using Atrio.Application;
using Atrio.Application.Common;
using Atrio.API.Services;
using Atrio.Infrastructure;
using Atrio.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var dotEnvPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (!File.Exists(dotEnvPath)) dotEnvPath = Path.Combine(Directory.GetCurrentDirectory(), "Atrio.API", ".env");
DotEnv.Load(dotEnvPath);
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

const string AllowFrontend = "AllowFrontend";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Atrio API",
        Version = "v1",
        Description = "Student Attendance Management System — Zynthra Technologies"
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
var jwtKey = builder.Configuration["Jwt:Key"] ?? builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT key is missing.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowFrontend, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Atrio API v1");
        options.RoutePrefix = "swagger";
    });
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (AppValidationException exception)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = exception.Message
        });
    }
});

app.UseHttpsRedirection();
app.UseCors(AllowFrontend);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
