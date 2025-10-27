// File: src/LockIn.Presentation.Wpf/Startup/ApplicationRegistration.cs
// (Añade esta extension en tu host builder existente; no se crean vistas nuevas)
namespace LockIn.Presentation.Wpf.Startup;

using LockIn.Application;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationRegistration
{
    public static IServiceCollection AddLockInLayers(this IServiceCollection services)
    {
        services.AddApplication();
        return services;
    }
}
