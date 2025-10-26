// File: src/LockIn.Domain/Services/PlanificacionPreview.cs
namespace LockIn.Domain.Services;

using System.Collections.Generic;
using System.Linq;

public sealed class PlanificacionPreview
{
    public IReadOnlyCollection<DateOnly> FechasSolicitadas { get; }
    public IReadOnlyCollection<DateOnly> FechasLibres { get; }
    public IReadOnlyCollection<DateOnly> FechasBloqueadas { get; }
    public bool TieneConflictos => FechasBloqueadas.Count > 0;

    public PlanificacionPreview(IEnumerable<DateOnly> solicitadas, IEnumerable<DateOnly> libres, IEnumerable<DateOnly> bloqueadas)
    {
        FechasSolicitadas = solicitadas.Distinct().OrderBy(x => x).ToList();
        FechasLibres = libres.Distinct().OrderBy(x => x).ToList();
        FechasBloqueadas = bloqueadas.Distinct().OrderBy(x => x).ToList();
    }
}
