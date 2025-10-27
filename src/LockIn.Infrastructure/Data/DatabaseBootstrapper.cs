using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using LockIn.Abstractions;
using LockIn.Infrastructure.Data.Sqlite;

namespace LockIn.Infrastructure.Data;

internal static class DatabaseBootstrapper
{
    public static void Initialize(IAppDataPathProvider paths, IAppReadOnlyState ro, ISqliteConnectionFactory factory)
    {
        paths.EnsureDataDirectory();

        // 1) Probar integridad antes de abrir EF
        var readOnly = false;
        try
        {
            using var conn = factory.Create(false);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA foreign_keys=ON; PRAGMA integrity_check;";
            using var r = cmd.ExecuteReader();
            string? result = null;
            while (r.Read()) result = r.GetString(0);

            if (!string.Equals(result, "ok", StringComparison.OrdinalIgnoreCase))
                readOnly = true;
        }
        catch
        {
            // no se pudo abrir o integrity_check falló → entrar en sólo-lectura
            readOnly = true;
        }

        ro.SetReadOnly(readOnly);

        // 2) Si no es read-only → migrar/crear esquema
        if (!readOnly)
        {
            var options = new DbContextOptionsBuilder<LockInDbContext>()
                .UseSqlite(factory.Create(false))
                .AddInterceptors(new SqlitePragmaInterceptor(false))
                .Options;

            using var ctx = new LockInDbContext(options);
            ctx.Database.Migrate();
            // Seeds mínimos (sin datos de usuario)
            InitialSeeds.Seed(ctx);
        }
    }
}
