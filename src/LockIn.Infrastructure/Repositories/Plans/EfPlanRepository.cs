using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LockIn.Infrastructure.Data;
using LockIn.Infrastructure.Data.Entities;
using LockIn.Infrastructure.Data.Enums;
using TaskStatus = LockIn.Infrastructure.Data.Enums.TaskStatus;

namespace LockIn.Infrastructure.Repositories.Plans;

public interface IPlanRepository
{
    Task AddAsync(Plan plan, IEnumerable<(DateOnly date, IEnumerable<DayTemplateItem> items)> days);
    Task<List<(DateOnly date, bool hasConflict)>> PreviewConflictsAsync(DateOnly start, DateOnly end);
    Task<List<PlanDay>> GetDaysAsync(Guid planId);
}

internal sealed class EfPlanRepository : IPlanRepository
{
    private readonly LockInDbContext _db;
    public EfPlanRepository(LockInDbContext db) => _db = db;

    public async Task<List<(DateOnly date, bool hasConflict)>> PreviewConflictsAsync(DateOnly start, DateOnly end)
    {
        var requested = Enumerable.Range(0, end.DayNumber - start.DayNumber + 1)
            .Select(i => start.AddDays(i)).ToList();

        var conflicted = await _db.PlanDays
            .Where(d => d.Date >= start && d.Date <= end)
            .Select(d => d.Date)
            .Distinct()
            .ToListAsync();

        return requested.Select(d => (d, conflicted.Contains(d))).ToList();
    }

    public async Task AddAsync(Plan plan, IEnumerable<(DateOnly date, IEnumerable<DayTemplateItem> items)> days)
    {
        await _db.Plans.AddAsync(plan);

        foreach (var (date, items) in days)
        {
            var pd = new PlanDay { Id = Guid.NewGuid(), PlanId = plan.Id, Date = date };
            _db.PlanDays.Add(pd);

            int order = 0;
            foreach (var it in items.OrderBy(i => i.StartTime).ThenBy(i => i.DisplayOrder))
            {
                var task = new PlanTask
                {
                    Id = Guid.NewGuid(),
                    PlanDayId = pd.Id,
                    TaskType = it.TaskType,
                    StartTime = it.StartTime,
                    DisplayOrder = ++order,
                    Status = TaskStatus.Rojo
                };

                // Snapshots para catálogos (Actividad Clave y Pastillas) :contentReference[oaicite:3]{index=3}.
                if (it.TaskType == TaskType.ActividadClave)
                {
                    // snapshot del nombre si existiera relación en plantilla (no la tiene; se captura al crear detalle)
                    task.KeyActivityNameSnapshot = null;
                }
                if (it.TaskType == TaskType.Pastillas && it.MedicationDoseId.HasValue)
                {
                    var dose = await _db.MedicationDoses.FindAsync(it.MedicationDoseId.Value);
                    if (dose != null)
                    {
                        task.MedicationNameSnapshot = dose.Name;
                        task.MedicationMgPerUnitSnapshot = dose.MgPerUnit;
                        task.MedicationUnitsPerDoseSnapshot = dose.UnitsPerDose;
                    }
                }

                _db.PlanTasks.Add(task);
            }
        }
    }

    public Task<List<PlanDay>> GetDaysAsync(Guid planId) =>
        _db.PlanDays.Include(x => x.Tasks).Where(x => x.PlanId == planId).ToListAsync();
}
