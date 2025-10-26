namespace LockIn.Domain.Entities;

using LockIn.Domain.Common;
using LockIn.Domain.ValueObjects;

public sealed class KeyActivityDefinition
{
    public KeyActivityId Id { get; }
    public string Nombre { get; }

    public KeyActivityDefinition(KeyActivityId id, string nombre)
    {
        Id = id;
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new KeyActivityNotEditableException()
            : nombre.Trim();
    }
}
