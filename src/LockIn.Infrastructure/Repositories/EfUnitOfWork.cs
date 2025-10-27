using System.Threading;
using System.Threading.Tasks;

namespace LockIn.Infrastructure.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

internal sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly Data.LockInDbContext _db;
    public EfUnitOfWork(Data.LockInDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
