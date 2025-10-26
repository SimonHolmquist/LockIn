// File: src/LockIn.Domain/Services/ReglasPlanificador.cs
namespace LockIn.Domain.Services;

using System.Collections.Generic;
using System.Linq;

public static class ReglasPlanificador
{
    public static PlanificacionPreview Previsualizar(IEnumerable<DateOnly> candidatos, IEnumerable<DateOnly> yaPlanificados)
    {
        var setPlanned = new HashSet<DateOnly>(yaPlanificados);
        var solicitadas = candidatos.Distinct().ToList();
        var libres = solicitadas.Where(d => !setPlanned.Contains(d)).ToList();
        var bloqueadas = solicitadas.Where(d => setPlanned.Contains(d)).ToList();
        return new PlanificacionPreview(solicitadas, libres, bloqueadas);
    }
}