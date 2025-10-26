// File: src/LockIn.Domain/Repositories/Repositories.cs
namespace LockIn.Domain.Repositories;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LockIn.Domain.Entities;
using LockIn.Domain.ValueObjects;

public interface IPlantillasRepo
{
    Task<PlantillaDeDia?> GetAsync(PlantillaId id, CancellationToken ct);
    Task AddAsync(PlantillaDeDia template, CancellationToken ct);
    Task UpdateAsync(PlantillaDeDia template, CancellationToken ct);
    Task DeleteAsync(PlantillaId id, CancellationToken ct);
    Task<bool> IsAssignedAsync(PlantillaId id, CancellationToken ct);
    Task<IReadOnlyCollection<PlantillaDeDia>> ListAsync(bool? soloConfirmadas, CancellationToken ct);
}

public interface IPlanesRepo
{
    Task<bool> ExistsForDateAsync(DateOnly date, CancellationToken ct);
    Task<Plan?> GetByDateAsync(DateOnly date, CancellationToken ct);
    Task AddAsync(Plan plan, CancellationToken ct);
    Task DeleteByDateAsync(DateOnly date, CancellationToken ct);
}

public interface IKeyActivitiesRepo
{
    Task AddAsync(KeyActivityDefinition def, CancellationToken ct);
    Task DeleteAsync(KeyActivityId id, CancellationToken ct);
    Task<IReadOnlyCollection<KeyActivityDefinition>> ListAsync(CancellationToken ct);
}

public interface IMedicationsRepo
{
    Task AddAsync(MedicationDefinition def, CancellationToken ct);
    Task DeleteAsync(MedicationId id, CancellationToken ct);
    Task<IReadOnlyCollection<MedicationDefinition>> ListAsync(CancellationToken ct);
}

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
