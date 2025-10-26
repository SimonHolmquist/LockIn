// File: src/LockIn.Domain/ValueObjects/Metrics.cs
namespace LockIn.Domain.ValueObjects;

using System;

public readonly record struct DistanciaKm
{
    public double Value { get; }
    public DistanciaKm(double value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Distancia debe ser >= 0");
        Value = value;
    }
    public static DistanciaKm Zero => new(0);
}

public readonly record struct Duracion
{
    public TimeSpan Value { get; }
    public Duracion(TimeSpan value)
    {
        if (value < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(value), "Duracion debe ser >= 0");
        Value = value;
    }
    public static Duracion Zero => new(TimeSpan.Zero);
}
