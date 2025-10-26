// File: tests/LockIn.Tests/Application/AppHandlersTests.cs
namespace LockIn.Tests.Application;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LockIn.Application.Plantillas;
using LockIn.Application.Planificador;
using LockIn.Application.DTOs;
using LockIn.Domain.Enums;
using LockIn.Tests.Fakes;
using Xunit;

public class AppHandlersTests
{
    [Fact]
    public async Task Crear_Confirmar_ClonarPlantilla_Y_PlanearPreview_Confirmar()
    {
        var plantRepo = new InMemoryPlantillasRepo();
        var planesRepo = new InMemoryPlanesRepo();
        var uow = new InMemoryUnitOfWork();

        var create = new Commands.CreateDayTemplate("Base");
        var createH = new Commands.CreateDayTemplateHandler(plantRepo, uow);
        var dto = await createH.Handle(create, CancellationToken.None);

        var addH = new Commands.AddBlockToTemplateHandler(plantRepo, uow);
        dto = await addH.Handle(new Commands.AddBlockToTemplate(dto.Id, new TimeOnly(7, 0), 0, TaskType.Desayuno), CancellationToken.None);
        dto = await addH.Handle(new Commands.AddBlockToTemplate(dto.Id, new TimeOnly(8, 0), 0, TaskType.Pastillas), CancellationToken.None);

        var confirmH = new Commands.ConfirmDayTemplateHandler(plantRepo, uow);
        dto = await confirmH.Handle(new Commands.ConfirmDayTemplate(dto.Id), CancellationToken.None);

        var cloneH = new Commands.CloneTemplateVersionHandler(plantRepo, uow);
        var clone = await cloneH.Handle(new Commands.CloneTemplateVersion(dto.Id), CancellationToken.None);
        Assert.Equal(dto.Version + 1, clone.Version);

        // Preview
        var previewH = new Planner.PreviewCreatePlansHandler(planesRepo, plantRepo);
        var sel = new DateSelectionDto(DateSelectionMode.Rango, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today).AddDays(2), null, null);
        var preview = await previewH.Handle(new Planner.PreviewCreatePlansFromTemplates(dto.Id, sel), CancellationToken.None);
        Assert.False(preview.TieneConflictos);
        Assert.Equal(3, preview.FechasLibres.Count);

        // Confirm
        var confirmPlanH = new Planner.ConfirmCreatePlansHandler(planesRepo, plantRepo, uow);
        var creados = await confirmPlanH.Handle(new Planner.ConfirmCreatePlansFromTemplates(dto.Id, sel, true), CancellationToken.None);
        Assert.Equal(3, creados.Count);
    }

    [Fact]
    public async Task Preview_Detecta_DiasBloqueados()
    {
        var plantRepo = new InMemoryPlantillasRepo();
        var planesRepo = new InMemoryPlanesRepo();
        var uow = new InMemoryUnitOfWork();

        // plantilla
        var createH = new Commands.CreateDayTemplateHandler(plantRepo, uow);
        var dto = await createH.Handle(new Commands.CreateDayTemplate("Tpl"), CancellationToken.None);
        var addH = new Commands.AddBlockToTemplateHandler(plantRepo, uow);
        await addH.Handle(new Commands.AddBlockToTemplate(dto.Id, new TimeOnly(7, 0), 0, TaskType.Desayuno), CancellationToken.None);
        await new Commands.ConfirmDayTemplateHandler(plantRepo, uow).Handle(new Commands.ConfirmDayTemplate(dto.Id), CancellationToken.None);

        var confirmPlanH = new Planner.ConfirmCreatePlansHandler(planesRepo, plantRepo, uow);
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        await confirmPlanH.Handle(new Planner.ConfirmCreatePlansFromTemplates(dto.Id,
            new DateSelectionDto(DateSelectionMode.Multiseleccion, null, null, new[] { hoy }, null), true), CancellationToken.None);

        var previewH = new Planner.PreviewCreatePlansHandler(planesRepo, plantRepo);
        var sel = new DateSelectionDto(DateSelectionMode.Multiseleccion, null, null, new[] { hoy, hoy.AddDays(1) }, null);
        var preview = await previewH.Handle(new Planner.PreviewCreatePlansFromTemplates(dto.Id, sel), CancellationToken.None);

        Assert.True(preview.TieneConflictos);
        Assert.Contains(hoy, preview.FechasBloqueadas);
        Assert.Contains(hoy.AddDays(1), preview.FechasLibres);
    }

    [Fact]
    public async Task Rendimiento_Confirmar_90Dias_10TareasPorDia_MenorA_500ms()
    {
        var plantRepo = new InMemoryPlantillasRepo();
        var planesRepo = new InMemoryPlanesRepo();
        var uow = new InMemoryUnitOfWork();

        var tpl = await new Commands.CreateDayTemplateHandler(plantRepo, uow)
            .Handle(new Commands.CreateDayTemplate("Perf"), CancellationToken.None);

        var addH = new Commands.AddBlockToTemplateHandler(plantRepo, uow);
        for (int i = 0; i < 10; i++)
            await addH.Handle(new Commands.AddBlockToTemplate(tpl.Id, new TimeOnly(6 + i, 0), 0, TaskType.Cinta), CancellationToken.None);
        await new Commands.ConfirmDayTemplateHandler(plantRepo, uow).Handle(new Commands.ConfirmDayTemplate(tpl.Id), CancellationToken.None);

        var start = DateOnly.FromDateTime(DateTime.Today);
        var end = start.AddDays(89);
        var sel = new DateSelectionDto(DateSelectionMode.Rango, start, end, null, null);

        var sw = Stopwatch.StartNew();
        var creados = await new Planner.ConfirmCreatePlansHandler(planesRepo, plantRepo, uow)
            .Handle(new Planner.ConfirmCreatePlansFromTemplates(tpl.Id, sel, true), CancellationToken.None);
        sw.Stop();

        Assert.Equal(90, creados.Count);
        Assert.True(sw.ElapsedMilliseconds < 500, $"Tardo {sw.ElapsedMilliseconds}ms");
    }
}
