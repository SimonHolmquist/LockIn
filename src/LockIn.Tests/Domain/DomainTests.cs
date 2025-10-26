// File: tests/LockIn.Tests/Domain/DomainTests.cs
namespace LockIn.Tests.Domain;

using System;
using System.Linq;
using LockIn.Domain.Entities;
using LockIn.Domain.Enums;
using LockIn.Domain.ValueObjects;
using Xunit;

public class DomainTests
{
    [Fact]
    public void Plantilla_Confirmada_SeVuelveInmutable()
    {
        var p = new PlantillaDeDia(PlantillaId.New(), "Base");
        p.AgregarBloque(new TimeOnly(7, 0), 0, TaskType.Desayuno);
        p.Confirmar();
        Assert.Throws<LockIn.Domain.Common.TemplateAlreadyConfirmedException>(() =>
            p.AgregarBloque(new TimeOnly(8, 0), 0, TaskType.Almuerzo));
    }

    [Fact]
    public void Ordenamiento_PorHoraYLuegoOrdenManual()
    {
        var p = new PlantillaDeDia(PlantillaId.New(), "Base");
        p.AgregarBloque(new TimeOnly(8, 0), 2, TaskType.Cinta);
        p.AgregarBloque(new TimeOnly(8, 0), 1, TaskType.Cinta);
        p.AgregarBloque(new TimeOnly(7, 0), 3, TaskType.Desayuno);
        var orden = p.Bloques.Select(b => (b.HoraInicio.Hour, b.OrdenManual)).ToArray();
        Assert.Equal(new[] { (7, 3), (8, 1), (8, 2) }, orden);
    }

    [Fact]
    public void Permite_MultiplesBloques_MismaHora()
    {
        var p = new PlantillaDeDia(PlantillaId.New(), "Plantilla");
        p.AgregarBloque(new TimeOnly(7, 0), 0, TaskType.Desayuno);
        p.AgregarBloque(new TimeOnly(7, 0), 1, TaskType.Pastillas);
        Assert.Equal(2, p.Bloques.Count(b => b.HoraInicio.Hour == 7));
    }

    [Fact]
    public void Snapshot_ActividadClave_SeGuardaEnPlanTask()
    {
        var t = new PlanTask(DateOnly.FromDateTime(DateTime.Today), TaskType.ActividadClave, new TimeOnly(10, 0));
        t.SnapshotActividadClave("Deep Work", 90);
        Assert.Equal("Deep Work", t.KeyActivityNameSnapshot);
        Assert.Equal(90, t.KeyActivityMinutes);
    }

    [Fact]
    public void Pastillas_ReglasDeEstado()
    {
        var t = new PlanTask(DateOnly.FromDateTime(DateTime.Today), TaskType.Pastillas, new TimeOnly(9, 0));
        t.SnapshotPastillas("Ibuprofeno", 400, 3);
        Assert.Equal(TaskStatus.Rojo, t.Estado);
        t.TomarUnaUnidad();
        Assert.Equal(TaskStatus.Amarillo, t.Estado);
        t.TomarUnaUnidad();
        t.TomarUnaUnidad();
        Assert.Equal(TaskStatus.Verde, t.Estado);
    }

    [Fact]
    public void Calculo_Progreso_Uniforme()
    {
        var d = DateOnly.FromDateTime(DateTime.Today);
        var a = new PlanTask(d, TaskType.Desayuno, new TimeOnly(8, 0));
        var b = new PlanTask(d, TaskType.Almuerzo, new TimeOnly(12, 0));
        var c = new PlanTask(d, TaskType.Cena, new TimeOnly(20, 0));
        a.SetEstadoManual(TaskStatus.Verde);
        b.SetEstadoManual(TaskStatus.Rojo);
        c.SetEstadoManual(TaskStatus.Verde);
        var p = new Plan(d);
        p.Agregar(a); p.Agregar(b); p.Agregar(c);
        var progreso = LockIn.Domain.Services.CalculadoraProgreso.Calcular(p.Tareas);
        Assert.Equal(2d / 3d, progreso, 3);
    }
}
