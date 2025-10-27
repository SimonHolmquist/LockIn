using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LockIn.Abstractions;
using LockIn.Infrastructure.Data;
using LockIn.Infrastructure.Data.Sqlite;
using LockIn.Infrastructure.Repositories;
using LockIn.Infrastructure.Repositories.Plans;
using LockIn.Infrastructure.Repositories.Templates;
using LockIn.Infrastructure.Repositories.Catalogs;

namespace LockIn.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureData(this IServiceCollection services)
    {
        services.AddSingleton<IAppDataPathProvider, AppDataPathProvider>();
        services.AddSingleton<IAppReadOnlyState, ReadOnlyState>();
        services.AddSingleton<ISqliteConnectionFactory, SqliteConnectionFactory>();

        // Bootstrap de base + integrity_check + PRAGMAs + migración cuando corresponde :contentReference[oaicite:5]{index=5}.
        using (var sp = services.BuildServiceProvider())
        {
            var paths = sp.GetRequiredService<IAppDataPathProvider>();
            var ro = sp.GetRequiredService<IAppReadOnlyState>();
            var factory = sp.GetRequiredService<ISqliteConnectionFactory>();
            DatabaseBootstrapper.Initialize(paths, ro, factory);
        }

        // Registrar DbContext con interceptores (respetando ReadOnly)
        services.AddDbContext<LockInDbContext>((sp, opt) =>
        {
            var ro = sp.GetRequiredService<IAppReadOnlyState>().ReadOnlyMode;
            var factory = sp.GetRequiredService<ISqliteConnectionFactory>();
            opt.UseSqlite(factory.Create(ro));
            opt.AddInterceptors(new SqlitePragmaInterceptor(ro));
        });

        // Repos + UoW
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IPlanRepository, EfPlanRepository>();
        services.AddScoped<ITemplateRepository, EfTemplateRepository>();
        services.AddScoped<IKeyActivityRepository, EfKeyActivityRepository>();
        services.AddScoped<IMedicationDoseRepository, EfMedicationDoseRepository>();

        return services;
    }
}
