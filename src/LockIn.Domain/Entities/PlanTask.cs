// File: src/LockIn.Domain/Entities/PlanTask.cs
namespace LockIn.Domain.Entities;

using LockIn.Domain.Common;
using LockIn.Domain.Enums;
using LockIn.Domain.ValueObjects;
using System;

public sealed class PlanTask
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateOnly Fecha { get; }
    public TaskType Tipo { get; }
    public TimeOnly? HoraInicio { get; }
    private TaskStatus _estadoManual;

    // Detalles por tipo
    public TrainingType? Training { get; private set; }

    public DistanciaKm Distancia { get; private set; } = DistanciaKm.Zero;
    public Duracion Duracion { get; private set; } = Duracion.Zero;

    public string? KeyActivityNameSnapshot { get; private set; }
    public int? KeyActivityMinutes { get; private set; }

    public string? MedicationNameSnapshot { get; private set; }
    public double? MedicationMgPorUnidadSnapshot { get; private set; }
    public int? MedicationUnidadesPorTomaSnapshot { get; private set; }
    public int? MedicationUnidadesTomadas { get; private set; }

    public PlanTask(DateOnly fecha, TaskType tipo, TimeOnly? horaInicio, TaskStatus estadoInicial = TaskStatus.Rojo)
    {
        Fecha = fecha;
        Tipo = tipo;
        HoraInicio = horaInicio;
        _estadoManual = estadoInicial;
    }

    public TaskStatus Estado =>
        Tipo == TaskType.Pastillas
            ? CalcularEstadoPastillas()
            : _estadoManual;

    public void SetEstadoManual(TaskStatus nuevo)
    {
        // Amarillo siempre manual salvo Pastillas; para Pastillas, estado deriva de unidades tomadas.
        if (Tipo == TaskType.Pastillas) return;
        _estadoManual = nuevo;
    }

    // Detalles Cinta
    public void SetCinta(DistanciaKm distancia, Duracion duracion)
    {
        if (distancia.Value < 0 || duracion.Value < TimeSpan.Zero) return;
        Distancia = distancia;
        Duracion = duracion;
    }
    public double? VelocidadMediaKmH => Duracion.Value == TimeSpan.Zero ? null : Distancia.Value / Duracion.Value.TotalHours;
    public TimeSpan? PaceMinPorKm => Distancia.Value <= 0 ? null : TimeSpan.FromMinutes(Duracion.Value.TotalMinutes / Distancia.Value);

    // Detalles Entrenamiento
    public void SetEntrenamiento(TrainingType tipo) => Training = tipo;

    // Detalles Actividad Clave
    public void SnapshotActividadClave(string nombre, int minutos)
    {
        KeyActivityNameSnapshot = string.IsNullOrWhiteSpace(nombre) ? throw new ArgumentException("Nombre requerido") : nombre.Trim();
        KeyActivityMinutes = minutos < 0 ? 0 : minutos;
    }

    // Detalles Pastillas
    public void SnapshotPastillas(string nombreGenerico, double mgPorUnidad, int unidadesPorToma)
    {
        if (string.IsNullOrWhiteSpace(nombreGenerico)) throw new ArgumentException("NombreGenerico requerido");
        if (mgPorUnidad <= 0) throw new MedicationInvalidDoseException("MgPorUnidad debe ser > 0");
        if (unidadesPorToma < 1) throw new MedicationInvalidDoseException("UnidadesPorToma debe ser >= 1");

        MedicationNameSnapshot = nombreGenerico.Trim();
        MedicationMgPorUnidadSnapshot = mgPorUnidad;
        MedicationUnidadesPorTomaSnapshot = unidadesPorToma;
        MedicationUnidadesTomadas = 0;
    }

    public void TomarUnaUnidad()
    {
        if (Tipo != TaskType.Pastillas) return;
        if (MedicationUnidadesTomadas is null || MedicationUnidadesPorTomaSnapshot is null) return;
        MedicationUnidadesTomadas = Math.Min(MedicationUnidadesPorTomaSnapshot.Value, MedicationUnidadesTomadas.Value + 1);
    }

    private TaskStatus CalcularEstadoPastillas()
    {
        if (MedicationUnidadesPorTomaSnapshot is null || MedicationUnidadesTomadas is null) return TaskStatus.Rojo;
        if (MedicationUnidadesTomadas == 0) return TaskStatus.Rojo;
        if (MedicationUnidadesTomadas < MedicationUnidadesPorTomaSnapshot) return TaskStatus.Amarillo; // Amarillo automatico solo aqui
        return TaskStatus.Verde;
    }
}