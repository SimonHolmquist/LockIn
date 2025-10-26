namespace LockIn.Domain.Entities;

using LockIn.Domain.Common;
using LockIn.Domain.ValueObjects;

public sealed class MedicationDefinition
{
    public MedicationId Id { get; }
    public string NombreGenerico { get; }
    public double MgPorUnidad { get; }
    public int UnidadesPorToma { get; }

    public MedicationDefinition(MedicationId id, string nombreGenerico, double mgPorUnidad, int unidadesPorToma)
    {
        if (string.IsNullOrWhiteSpace(nombreGenerico))
            throw new MedicationInvalidDoseException("NombreGenerico requerido");
        if (mgPorUnidad <= 0)
            throw new MedicationInvalidDoseException("MgPorUnidad debe ser > 0");
        if (unidadesPorToma < 1)
            throw new MedicationInvalidDoseException("UnidadesPorToma debe ser >= 1");

        Id = id;
        NombreGenerico = nombreGenerico.Trim();
        MgPorUnidad = mgPorUnidad;
        UnidadesPorToma = unidadesPorToma;
    }
}
