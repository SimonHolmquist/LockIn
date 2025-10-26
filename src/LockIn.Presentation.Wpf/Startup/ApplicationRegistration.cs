// File: src/LockIn.Presentation.Wpf/Startup/ApplicationRegistration.cs
// (Añade esta extension en tu host builder existente; no se crean vistas nuevas)
namespace LockIn.Presentation.Wpf.Startup;

using Microsoft.Extensions.DependencyInjection;
using LockIn.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddLockInLayers(this IServiceCollection services)
    {
        // Domain no requiere DI por ahora
        services.AddApplication();
        return services;
    }
}
