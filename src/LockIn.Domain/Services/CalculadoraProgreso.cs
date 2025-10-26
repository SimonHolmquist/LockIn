// File: src/LockIn.Domain/Services/CalculadoraProgreso.cs
namespace LockIn.Domain.Services;

using System.Collections.Generic;
using System.Linq;
using LockIn.Domain.Entities;
using LockIn.Domain.Enums;

public static class CalculadoraProgreso
{
    // Retorna [0..1] ponderando uniforme por cantidad de tareas del dia
    public static double Calcular(IEnumerable<PlanTask> tareas)
    {
        var list = tareas?.ToList() ?? new();
        if (list.Count == 0) return 0d;
        var verdes = list.Count(t => t.Estado == TaskStatus.Verde);
        return verdes / (double)list.Count;
    }
}
