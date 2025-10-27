using System.Linq;

namespace LockIn.Infrastructure.Data;

internal static class InitialSeeds
{
    public static void Seed(LockInDbContext db)
    {
        // Sin catálogos pre-cargados para no “ensuciar” MVP (opcional: ejemplo desactivado) :contentReference[oaicite:2]{index=2}.
        // Ejemplo desactivado por defecto (dejar comentado):
        // if (!db.DayTemplates.Any()) { /* crear una plantilla de ejemplo */ }
        db.SaveChanges();
    }
}
