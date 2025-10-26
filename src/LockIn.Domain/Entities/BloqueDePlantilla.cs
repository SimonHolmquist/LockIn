// File: src/LockIn.Domain/Entities/BloqueDePlantilla.cs
namespace LockIn.Domain.Entities;

using LockIn.Domain.Common;
using LockIn.Domain.Enums;
using LockIn.Domain.ValueObjects;
using System;

public sealed class BloqueDePlantilla
{
    public BloqueId Id { get; }
    public TimeOnly HoraInicio { get; private set; }
    public int OrdenManual { get; private set; }
    public TaskType Tipo { get; }

    public BloqueDePlantilla(BloqueId id, TimeOnly horaInicio, int ordenManual, TaskType tipo)
    {
        if (ordenManual < 0) throw new InvalidManualOrderException();
        Id = id;
        HoraInicio = horaInicio;
        OrdenManual = ordenManual;
        Tipo = tipo;
    }

    internal void Reordenar(int nuevoOrden)
    {
        if (nuevoOrden < 0) throw new InvalidManualOrderException();
        OrdenManual = nuevoOrden;
    }
}
