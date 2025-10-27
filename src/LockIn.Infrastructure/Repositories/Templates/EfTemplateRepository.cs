using LockIn.Infrastructure.Data;
using LockIn.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LockIn.Infrastructure.Repositories.Templates;

public interface ITemplateRepository
{
    Task<DayTemplate?> GetAsync(Guid id);
    Task AddAsync(DayTemplate template);
    IQueryable<DayTemplate> Query();
}

internal sealed class EfTemplateRepository : ITemplateRepository
{
    private readonly LockInDbContext _db;
    public EfTemplateRepository(LockInDbContext db) => _db = db;

    public Task<DayTemplate?> GetAsync(Guid id) =>
        _db.DayTemplates.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(DayTemplate template) => await _db.DayTemplates.AddAsync(template);

    public IQueryable<DayTemplate> Query() => _db.DayTemplates.AsNoTracking();
}
