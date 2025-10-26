// File: src/LockIn.Domain/Entities/PlantillaDeDia.cs
namespace LockIn.Domain.Entities;

using LockIn.Domain.Common;
using LockIn.Domain.Enums;
using LockIn.Domain.ValueObjects;
using System;

public sealed class PlantillaDeDia
{
    private readonly List<BloqueDePlantilla> _bloques = new();
    public PlantillaId Id { get; }
    public string Nombre { get; private set; }
    public int Version { get; private set; }
    public TemplateStatus Estado { get; private set; }

    public IReadOnlyCollection<BloqueDePlantilla> Bloques => _bloques.OrderBy(b => b.HoraInicio).ThenBy(b => b.OrdenManual).ToList();

    public PlantillaDeDia(PlantillaId id, string nombre, int version = 1)
    {
        Id = id;
        Nombre = string.IsNullOrWhiteSpace(nombre) ? throw new ArgumentException("Nombre requerido") : nombre.Trim();
        Version = version <= 0 ? 1 : version;
        Estado = TemplateStatus.Borrador;
    }

    public void Confirmar()
    {
        if (Estado == TemplateStatus.Confirmada) throw new TemplateAlreadyConfirmedException();
        Estado = TemplateStatus.Confirmada;
    }

    public PlantillaDeDia ClonarNuevaVersion()
    {
        var clone = new PlantillaDeDia(PlantillaId.New(), Nombre, Version + 1);
        foreach (var b in _bloques)
            clone._bloques.Add(new BloqueDePlantilla(BloqueId.New(), b.HoraInicio, b.OrdenManual, b.Tipo));
        return clone;
    }

    public void AgregarBloque(TimeOnly hora, int orden, TaskType tipo)
    {
        AsegurarEditable();
        _bloques.Add(new BloqueDePlantilla(BloqueId.New(), hora, orden, tipo));
    }

    public void ReordenarBloque(BloqueId bloqueId, int nuevoOrden)
    {
        AsegurarEditable();
        var b = _bloques.Single(x => x.Id.Equals(bloqueId));
        b.Reordenar(nuevoOrden);
    }

    private void AsegurarEditable()
    {
        if (Estado == TemplateStatus.Confirmada) throw new TemplateAlreadyConfirmedException();
    }
}

