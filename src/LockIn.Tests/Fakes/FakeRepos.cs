// File: tests/LockIn.Tests/Fakes/FakeRepos.cs
namespace LockIn.Tests.Fakes;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LockIn.Domain.Entities;
using LockIn.Domain.Repositories;
using LockIn.Domain.ValueObjects;

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
}

public sealed class InMemoryPlantillasRepo : IPlantillasRepo
{
    private readonly ConcurrentDictionary<Guid, PlantillaDeDia> _store = new();
    private readonly HashSet<Guid> _asignadas = new(); // simulate

    public Task AddAsync(PlantillaDeDia template, CancellationToken ct)
    { _store[template.Id.Value] = template; return Task.CompletedTask; }

    public Task DeleteAsync(PlantillaId id, CancellationToken ct)
    { _store.TryRemove(id.Value, out _); return Task.CompletedTask; }

    public Task<PlantillaDeDia?> GetAsync(PlantillaId id, CancellationToken ct)
    { _store.TryGetValue(id.Value, out var t); return Task.FromResult(t); }

    public Task<bool> IsAssignedAsync(PlantillaId id, CancellationToken ct)
    { return Task.FromResult(_asignadas.Contains(id.Value)); }

    public Task<IReadOnlyCollection<PlantillaDeDia>> ListAsync(bool? soloConfirmadas, CancellationToken ct)
    {
        var list = _store.Values.AsEnumerable();
        if (soloConfirmadas is true) list = list.Where(t => t.Estado == LockIn.Domain.Enums.TemplateStatus.Confirmada);
        if (soloConfirmadas is false) list = list.Where(t => t.Estado == LockIn.Domain.Enums.TemplateStatus.Borrador);
        return Task.FromResult((IReadOnlyCollection<PlantillaDeDia>)list.ToList());
    }

    public Task UpdateAsync(PlantillaDeDia template, CancellationToken ct)
    { _store[template.Id.Value] = template; return Task.CompletedTask; }

    // helper para tests
    public void MarcarAsignada(PlantillaId id) => _asignadas.Add(id.Value);
}

public sealed class InMemoryPlanesRepo : IPlanesRepo
{
    private readonly ConcurrentDictionary<DateOnly, Plan> _byDate = new();

    public Task AddAsync(Plan plan, CancellationToken ct)
    { _byDate[plan.Fecha] = plan; return Task.CompletedTask; }

    public Task DeleteByDateAsync(DateOnly date, CancellationToken ct)
    { _byDate.TryRemove(date, out _); return Task.CompletedTask; }

    public Task<bool> ExistsForDateAsync(DateOnly date, CancellationToken ct)
    { return Task.FromResult(_byDate.ContainsKey(date)); }

    public Task<Plan?> GetByDateAsync(DateOnly date, CancellationToken ct)
    { _byDate.TryGetValue(date, out var p); return Task.FromResult(p); }
}

public sealed class InMemoryKeyActivitiesRepo : IKeyActivitiesRepo
{
    private readonly ConcurrentDictionary<Guid, KeyActivityDefinition> _store = new();
    public Task AddAsync(KeyActivityDefinition def, CancellationToken ct) { _store[def.Id.Value] = def; return Task.CompletedTask; }
    public Task DeleteAsync(KeyActivityId id, CancellationToken ct) { _store.TryRemove(id.Value, out _); return Task.CompletedTask; }
    public Task<IReadOnlyCollection<KeyActivityDefinition>> ListAsync(CancellationToken ct) => Task.FromResult((IReadOnlyCollection<KeyActivityDefinition>)_store.Values.ToList());
}

public sealed class InMemoryMedicationsRepo : IMedicationsRepo
{
    private readonly ConcurrentDictionary<Guid, MedicationDefinition> _store = new();
    public Task AddAsync(MedicationDefinition def, CancellationToken ct) { _store[def.Id.Value] = def; return Task.CompletedTask; }
    public Task DeleteAsync(MedicationId id, CancellationToken ct) { _store.TryRemove(id.Value, out _); return Task.CompletedTask; }
    public Task<IReadOnlyCollection<MedicationDefinition>> ListAsync(CancellationToken ct) => Task.FromResult((IReadOnlyCollection<MedicationDefinition>)_store.Values.ToList());
}
