using HOMMS.Application.Interfaces;
using HOMMS.Infrastructure.Services;
using HOMMS.API.Middleware;
using AutoMapper;
using System.Reflection;

public static class ServiceExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        // Register AutoMapper (scan only relevant assemblies to avoid System.Windows.Forms error)
        var assemblies = new[]
        {
            Assembly.Load("HOMMS.Application"),
            Assembly.Load("HOMMS.Domain"),
            Assembly.Load("HOMMS.API")
        };
        services.AddAutoMapper(assemblies);
        // Register other custom services here
    }
} 