// File: src/LockIn.Application/Plantillas/Commands.cs
namespace LockIn.Application.Plantillas;

using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using LockIn.Domain.Entities;
using LockIn.Domain.Repositories;
using LockIn.Domain.ValueObjects;
using LockIn.Application.DTOs;
using LockIn.Application.Common;
using LockIn.Domain.Enums;
using LockIn.Domain.Common;

public static class Commands
{
    // CreateDayTemplate
    public sealed record CreateDayTemplate(string Nombre) : IRequest<PlantillaDto>;
    public sealed class CreateDayTemplateValidator : AbstractValidator<CreateDayTemplate>
    {
        public CreateDayTemplateValidator() => RuleFor(x => x.Nombre).NotEmpty();
    }
    public sealed class CreateDayTemplateHandler : IRequestHandler<CreateDayTemplate, PlantillaDto>
    {
        private readonly IPlantillasRepo _repo;
        private readonly IUnitOfWork _uow;
        public CreateDayTemplateHandler(IPlantillasRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<PlantillaDto> Handle(CreateDayTemplate r, CancellationToken ct)
        {
            var t = new PlantillaDeDia(PlantillaId.New(), r.Nombre, 1);
            await _repo.AddAsync(t, ct);
            await _uow.SaveChangesAsync(ct);
            return t.ToDto();
        }
    }

    // AddBlockToTemplate
    public sealed record AddBlockToTemplate(Guid PlantillaId, TimeOnly HoraInicio, int OrdenManual, TaskType Tipo) : IRequest<PlantillaDto>;
    public sealed class AddBlockToTemplateValidator : AbstractValidator<AddBlockToTemplate>
    {
        public AddBlockToTemplateValidator()
        {
            RuleFor(x => x.PlantillaId).NotEmpty();
            RuleFor(x => x.OrdenManual).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class AddBlockToTemplateHandler : IRequestHandler<AddBlockToTemplate, PlantillaDto>
    {
        private readonly IPlantillasRepo _repo; private readonly IUnitOfWork _uow;
        public AddBlockToTemplateHandler(IPlantillasRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<PlantillaDto> Handle(AddBlockToTemplate r, CancellationToken ct)
        {
            var id = new PlantillaId(r.PlantillaId);
            var t = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException("Plantilla no encontrada");
            t.AgregarBloque(r.HoraInicio, r.OrdenManual, r.Tipo);
            await _repo.UpdateAsync(t, ct);
            await _uow.SaveChangesAsync(ct);
            return t.ToDto();
        }
    }

    // ReorderBlockInTemplate
    public sealed record ReorderBlockInTemplate(Guid PlantillaId, Guid BloqueId, int NuevoOrden) : IRequest<PlantillaDto>;
    public sealed class ReorderBlockInTemplateValidator : AbstractValidator<ReorderBlockInTemplate>
    {
        public ReorderBlockInTemplateValidator()
        {
            RuleFor(x => x.PlantillaId).NotEmpty();
            RuleFor(x => x.BloqueId).NotEmpty();
            RuleFor(x => x.NuevoOrden).GreaterThanOrEqualTo(0);
        }
    }
    public sealed class ReorderBlockInTemplateHandler : IRequestHandler<ReorderBlockInTemplate, PlantillaDto>
    {
        private readonly IPlantillasRepo _repo; private readonly IUnitOfWork _uow;
        public ReorderBlockInTemplateHandler(IPlantillasRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<PlantillaDto> Handle(ReorderBlockInTemplate r, CancellationToken ct)
        {
            var t = await _repo.GetAsync(new PlantillaId(r.PlantillaId), ct) ?? throw new KeyNotFoundException();
            t.ReordenarBloque(new LockIn.Domain.ValueObjects.BloqueId(r.BloqueId), r.NuevoOrden);
            await _repo.UpdateAsync(t, ct);
            await _uow.SaveChangesAsync(ct);
            return t.ToDto();
        }
    }

    // ConfirmDayTemplate
    public sealed record ConfirmDayTemplate(Guid PlantillaId) : IRequest<PlantillaDto>;
    public sealed class ConfirmDayTemplateHandler : IRequestHandler<ConfirmDayTemplate, PlantillaDto>
    {
        private readonly IPlantillasRepo _repo; private readonly IUnitOfWork _uow;
        public ConfirmDayTemplateHandler(IPlantillasRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<PlantillaDto> Handle(ConfirmDayTemplate r, CancellationToken ct)
        {
            var t = await _repo.GetAsync(new PlantillaId(r.PlantillaId), ct) ?? throw new KeyNotFoundException();
            t.Confirmar();
            await _repo.UpdateAsync(t, ct);
            await _uow.SaveChangesAsync(ct);
            return t.ToDto();
        }
    }

    // CloneTemplateVersion
    public sealed record CloneTemplateVersion(Guid PlantillaId) : IRequest<PlantillaDto>;
    public sealed class CloneTemplateVersionHandler : IRequestHandler<CloneTemplateVersion, PlantillaDto>
    {
        private readonly IPlantillasRepo _repo; private readonly IUnitOfWork _uow;
        public CloneTemplateVersionHandler(IPlantillasRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<PlantillaDto> Handle(CloneTemplateVersion r, CancellationToken ct)
        {
            var t = await _repo.GetAsync(new PlantillaId(r.PlantillaId), ct) ?? throw new KeyNotFoundException();
            var clone = t.ClonarNuevaVersion();
            await _repo.AddAsync(clone, ct);
            await _uow.SaveChangesAsync(ct);
            return clone.ToDto();
        }
    }

    // DeleteTemplate (si no asignada)
    public sealed record DeleteTemplate(Guid PlantillaId) : IRequest<Unit>;
    public sealed class DeleteTemplateHandler : IRequestHandler<DeleteTemplate, Unit>
    {
        private readonly IPlantillasRepo _repo; private readonly IUnitOfWork _uow;
        public DeleteTemplateHandler(IPlantillasRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<Unit> Handle(DeleteTemplate r, CancellationToken ct)
        {
            var id = new PlantillaId(r.PlantillaId);
            if (await _repo.IsAssignedAsync(id, ct)) throw new TemplateWithAssignmentsCannotBeDeletedException();
            await _repo.DeleteAsync(id, ct);
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
