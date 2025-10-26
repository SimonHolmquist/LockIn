// File: src/LockIn.Application/Common/MappingExtensions.cs
namespace LockIn.Application.Common;

using LockIn.Application.DTOs;
using LockIn.Domain.Entities;
using System.Linq;

public static class MappingExtensions
{
    public static PlantillaDto ToDto(this PlantillaDeDia t) =>
        new(t.Id.Value, t.Nombre, t.Version, t.Estado.ToString(),
            t.Bloques.Select(b => new BloqueDto(b.Id.Value, b.HoraInicio, b.OrdenManual, b.Tipo)).ToList());

    public static PlanDto ToDto(this Plan p) =>
        new(p.Id, p.Fecha, p.Estado.ToString(),
            p.Tareas.Select(t => new PlanTaskDto(t.Id, t.Fecha, t.Tipo, t.HoraInicio, t.Estado.ToString())).ToList());
}
