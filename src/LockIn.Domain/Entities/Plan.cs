// File: src/LockIn.Domain/Entities/Plan.cs
namespace LockIn.Domain.Entities;

using LockIn.Domain.Common;
using LockIn.Domain.Enums;
using System;

public sealed class Plan
{
    private readonly List<PlanTask> _tareas = new();
    public Guid Id { get; } = Guid.NewGuid();
    public DateOnly Fecha { get; }
    public PlanStatus Estado { get; private set; } = PlanStatus.Borrador;

    public IReadOnlyCollection<PlanTask> Tareas => _tareas
        .OrderBy(t => t.HoraInicio ?? new TimeOnly(0, 0))
        .ThenBy(t => t.Tipo).ToList();

    public Plan(DateOnly fecha) => Fecha = fecha;

    public void Agregar(PlanTask t)
    {
        AsegurarEditable();
        _tareas.Add(t);
    }

    public void Confirmar()
    {
        if (Estado == PlanStatus.Confirmado) throw new PlanAlreadyConfirmedException();
        Estado = PlanStatus.Confirmado;
    }

    private void AsegurarEditable()
    {
        if (Estado == PlanStatus.Confirmado) throw new PlanAlreadyConfirmedException();
    }

    public static Plan DesdePlantilla(PlantillaDeDia plantilla, DateOnly fecha)
    {
        var plan = new Plan(fecha);
        foreach (var b in plantilla.Bloques)
            plan.Agregar(new PlanTask(fecha, b.Tipo, b.HoraInicio));
        return plan;
    }
}