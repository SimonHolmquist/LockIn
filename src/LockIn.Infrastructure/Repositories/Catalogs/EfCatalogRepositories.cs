using LockIn.Infrastructure.Data;
using LockIn.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LockIn.Infrastructure.Repositories.Catalogs;

public interface IKeyActivityRepository
{
    Task AddAsync(KeyActivity e);
    Task DeleteAsync(Guid id); // se puede eliminar pero no editar (snapshot preserva nombre) :contentReference[oaicite:4]{index=4}
    IQueryable<KeyActivity> Query();
}

public interface IMedicationDoseRepository
{
    Task AddAsync(MedicationDose e);
    IQueryable<MedicationDose> Query();
}

internal sealed class EfKeyActivityRepository : IKeyActivityRepository
{
    private readonly LockInDbContext _db;
    public EfKeyActivityRepository(LockInDbContext db) => _db = db;

    public Task AddAsync(KeyActivity e) => _db.KeyActivities.AddAsync(e).AsTask();

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.KeyActivities.FindAsync(id);
        if (entity != null) _db.KeyActivities.Remove(entity);
    }

    public IQueryable<KeyActivity> Query() => _db.KeyActivities.AsNoTracking();
}

internal sealed class EfMedicationDoseRepository : IMedicationDoseRepository
{
    private readonly LockInDbContext _db;
    public EfMedicationDoseRepository(LockInDbContext db) => _db = db;

    public Task AddAsync(MedicationDose e) => _db.MedicationDoses.AddAsync(e).AsTask();

    public IQueryable<MedicationDose> Query() => _db.MedicationDoses.AsNoTracking();
}
