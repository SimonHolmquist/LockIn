using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LockIn.Infrastructure.Data;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LockInDbContext>
{
    public LockInDbContext CreateDbContext(string[] args)
    {
        // Ruta real de la app: %AppData%\LockIn\lockin.db
        var dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LockIn");
        Directory.CreateDirectory(dataDir);
        var dbPath = Path.Combine(dataDir, "lockin.db");

        var options = new DbContextOptionsBuilder<LockInDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            // No necesitamos interceptores ni DI para migrar
            .Options;

        return new LockInDbContext(options);
    }
}
