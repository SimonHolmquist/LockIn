// File: src/LockIn.Application/Planificador/CommandsQueries.cs
namespace LockIn.Application.Planificador;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using LockIn.Application.DTOs;
using LockIn.Domain.Repositories;
using LockIn.Domain.ValueObjects;
using LockIn.Domain.Services;
using LockIn.Application.Common;
using LockIn.Domain.Entities;
using LockIn.Domain.Common;

public static class Planner
{
    // Build selected dates
    private static IEnumerable<DateOnly> Expand(DateSelectionDto s)
    {
        return s.Mode switch
        {
            DateSelectionMode.Rango => Enumerable.Range(0, (s.End!.Value.DayNumber - s.Start!.Value.DayNumber) + 1)
                                                 .Select(offset => s.Start!.Value.AddDays(offset)),
            DateSelectionMode.Multiseleccion => s.Dates ?? Array.Empty<DateOnly>(),
            DateSelectionMode.Patron => Enumerable.Range(0, (s.End!.Value.DayNumber - s.Start!.Value.DayNumber) + 1)
                                                  .Select(i => s.Start!.Value.AddDays(i))
                                                  .Where(d => s.PatternDays!.Contains(d.DayOfWeek)),
            _ => Array.Empty<DateOnly>()
        };
    }

    // Preview
    public sealed record PreviewCreatePlansFromTemplates(Guid PlantillaId, DateSelectionDto Seleccion) : IRequest<PlanPreviewResultDto>;
    public sealed class PreviewCreatePlansValidator : AbstractValidator<PreviewCreatePlansFromTemplates>
    {
        public PreviewCreatePlansValidator()
        {
            RuleFor(x => x.PlantillaId).NotEmpty();
            RuleFor(x => x.Seleccion).NotNull();
        }
    }
    public sealed class PreviewCreatePlansHandler : IRequestHandler<PreviewCreatePlansFromTemplates, PlanPreviewResultDto>
    {
        private readonly IPlanesRepo _planes;
        private readonly IPlantillasRepo _plantillas;
        public PreviewCreatePlansHandler(IPlanesRepo planes, IPlantillasRepo plantillas) { _planes = planes; _plantillas = plantillas; }

        public async Task<PlanPreviewResultDto> Handle(PreviewCreatePlansFromTemplates r, CancellationToken ct)
        {
            _ = await _plantillas.GetAsync(new PlantillaId(r.PlantillaId), ct) ?? throw new KeyNotFoundException("Plantilla no encontrada");

            var fechas = Expand(r.Seleccion).Distinct().OrderBy(d => d).ToList();
            var ya = new List<DateOnly>();
            foreach (var d in fechas)
                if (await _planes.ExistsForDateAsync(d, ct)) ya.Add(d);

            var preview = ReglasPlanificador.Previsualizar(fechas, ya);
            return new PlanPreviewResultDto(preview.FechasSolicitadas, preview.FechasLibres, preview.FechasBloqueadas, preview.TieneConflictos);
        }
    }

    // Confirm (crear y confirmar en lote)
    public sealed record ConfirmCreatePlansFromTemplates(Guid PlantillaId, DateSelectionDto Seleccion, bool ContinuarYSaltarBloqueados) : IRequest<IReadOnlyCollection<PlanDto>>;
    public sealed class ConfirmCreatePlansValidator : AbstractValidator<ConfirmCreatePlansFromTemplates>
    {
        public ConfirmCreatePlansValidator()
        {
            RuleFor(x => x.PlantillaId).NotEmpty();
            RuleFor(x => x.Seleccion).NotNull();
        }
    }
    public sealed class ConfirmCreatePlansHandler : IRequestHandler<ConfirmCreatePlansFromTemplates, IReadOnlyCollection<PlanDto>>
    {
        private readonly IPlanesRepo _planes; private readonly IPlantillasRepo _plantillas; private readonly IUnitOfWork _uow;
        public ConfirmCreatePlansHandler(IPlanesRepo planes, IPlantillasRepo plantillas, IUnitOfWork uow) { _planes = planes; _plantillas = plantillas; _uow = uow; }

        public async Task<IReadOnlyCollection<PlanDto>> Handle(ConfirmCreatePlansFromTemplates r, CancellationToken ct)
        {
            var plantilla = await _plantillas.GetAsync(new PlantillaId(r.PlantillaId), ct) ?? throw new KeyNotFoundException("Plantilla no encontrada");
            var fechas = Expand(r.Seleccion).Distinct().OrderBy(d => d).ToList();

            var bloqueadas = new List<DateOnly>();
            foreach (var d in fechas)
                if (await _planes.ExistsForDateAsync(d, ct)) bloqueadas.Add(d);

            if (bloqueadas.Any() && !r.ContinuarYSaltarBloqueados)
                throw new OverlappingPlanRangeException();

            var aCrear = fechas.Except(bloqueadas).ToList();
            var creados = new List<PlanDto>();
            foreach (var d in aCrear)
            {
                var plan = Plan.DesdePlantilla(plantilla, d);
                plan.Confirmar();
                await _planes.AddAsync(plan, ct);
                creados.Add(plan.ToDto());
            }
            await _uow.SaveChangesAsync(ct);
            return creados;
        }
    }

    // Delete plan (por fecha)
    public sealed record DeletePlan(DateOnly Fecha) : IRequest<Unit>;
    public sealed class DeletePlanHandler : IRequestHandler<DeletePlan, Unit>
    {
        private readonly IPlanesRepo _planes; private readonly IUnitOfWork _uow;
        public DeletePlanHandler(IPlanesRepo planes, IUnitOfWork uow) { _planes = planes; _uow = uow; }
        public async Task<Unit> Handle(DeletePlan r, CancellationToken ct)
        {
            await _planes.DeleteByDateAsync(r.Fecha, ct);
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }

    // Get plan by date
    public sealed record GetPlanByDate(DateOnly Fecha) : IRequest<PlanDto?>;
    public sealed class GetPlanByDateHandler : IRequestHandler<GetPlanByDate, PlanDto?>
    {
        private readonly IPlanesRepo _planes;
        public GetPlanByDateHandler(IPlanesRepo planes) => _planes = planes;
        public async Task<PlanDto?> Handle(GetPlanByDate r, CancellationToken ct)
            => (await _planes.GetByDateAsync(r.Fecha, ct))?.ToDto();
    }

    // Get preview conflicts (alias para recomputar)
    public sealed record GetPlanPreviewConflicts(Guid PlantillaId, DateSelectionDto Seleccion) : IRequest<PlanPreviewResultDto>;
    public sealed class GetPlanPreviewConflictsHandler : IRequestHandler<GetPlanPreviewConflicts, PlanPreviewResultDto>
    {
        private readonly IMediator _mediator;
        public GetPlanPreviewConflictsHandler(IMediator mediator) => _mediator = mediator;
        public Task<PlanPreviewResultDto> Handle(GetPlanPreviewConflicts r, CancellationToken ct)
            => _mediator.Send(new PreviewCreatePlansFromTemplates(r.PlantillaId, r.Seleccion), ct);
    }

    // GetPlanDayProgress
    public sealed record GetPlanDayProgress(DateOnly Fecha) : IRequest<double>;
    public sealed class GetPlanDayProgressHandler : IRequestHandler<GetPlanDayProgress, double>
    {
        private readonly IPlanesRepo _planes;
        public GetPlanDayProgressHandler(IPlanesRepo planes) => _planes = planes;
        public async Task<double> Handle(GetPlanDayProgress r, CancellationToken ct)
        {
            var plan = await _planes.GetByDateAsync(r.Fecha, ct);
            if (plan is null) return 0;
            return LockIn.Domain.Services.CalculadoraProgreso.Calcular(plan.Tareas);
        }
    }
}
