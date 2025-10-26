// File: src/LockIn.Domain/ValueObjects/Ids.cs
namespace LockIn.Domain.ValueObjects;

using System;

public readonly record struct DiaId
{
    public Guid Value { get; }
    public DiaId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("DiaId vacio", nameof(value));
        Value = value;
    }
    public static DiaId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct PlanId
{
    public Guid Value { get; }
    public PlanId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("PlanId vacio", nameof(value));
        Value = value;
    }
    public static PlanId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct PlantillaId
{
    public Guid Value { get; }
    public PlantillaId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("PlantillaId vacio", nameof(value));
        Value = value;
    }
    public static PlantillaId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct BloqueId
{
    public Guid Value { get; }
    public BloqueId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("BloqueId vacio", nameof(value));
        Value = value;
    }
    public static BloqueId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct KeyActivityId
{
    public Guid Value { get; }
    public KeyActivityId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("KeyActivityId vacio", nameof(value));
        Value = value;
    }
    public static KeyActivityId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct MedicationId
{
    public Guid Value { get; }
    public MedicationId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("MedicationId vacio", nameof(value));
        Value = value;
    }
    public static MedicationId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
