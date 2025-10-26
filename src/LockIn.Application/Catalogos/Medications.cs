// File: src/LockIn.Application/Catalogos/Medications.cs
namespace LockIn.Application.Catalogos;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using LockIn.Domain.Entities;
using LockIn.Domain.Repositories;
using LockIn.Domain.ValueObjects;

public static class Medications
{
    public sealed record CreateMedication(string NombreGenerico, double MgPorUnidad, int UnidadesPorToma) : IRequest<Unit>;
    public sealed class CreateMedicationValidator : AbstractValidator<CreateMedication>
    {
        public CreateMedicationValidator()
        {
            RuleFor(x => x.NombreGenerico).NotEmpty();
            RuleFor(x => x.MgPorUnidad).GreaterThan(0);
            RuleFor(x => x.UnidadesPorToma).GreaterThanOrEqualTo(1);
        }
    }
    public sealed class CreateMedicationHandler : IRequestHandler<CreateMedication, Unit>
    {
        private readonly IMedicationsRepo _repo; private readonly IUnitOfWork _uow;
        public CreateMedicationHandler(IMedicationsRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<Unit> Handle(CreateMedication r, CancellationToken ct)
        {
            await _repo.AddAsync(new MedicationDefinition(MedicationId.New(), r.NombreGenerico, r.MgPorUnidad, r.UnidadesPorToma), ct);
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }

    public sealed record DeleteMedication(Guid Id) : IRequest<Unit>;
    public sealed class DeleteMedicationHandler : IRequestHandler<DeleteMedication, Unit>
    {
        private readonly IMedicationsRepo _repo; private readonly IUnitOfWork _uow;
        public DeleteMedicationHandler(IMedicationsRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<Unit> Handle(DeleteMedication r, CancellationToken ct)
        {
            await _repo.DeleteAsync(new MedicationId(r.Id), ct);
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }

    public sealed record GetMedications() : IRequest<IReadOnlyCollection<(Guid Id, string Nombre, double MgPorUnidad, int UnidadesPorToma)>>;
    public sealed class GetMedicationsHandler : IRequestHandler<GetMedications, IReadOnlyCollection<(Guid, string, double, int)>>
    {
        private readonly IMedicationsRepo _repo;
        public GetMedicationsHandler(IMedicationsRepo repo) => _repo = repo;
        public async Task<IReadOnlyCollection<(Guid, string, double, int)>> Handle(GetMedications r, CancellationToken ct)
            => (await _repo.ListAsync(ct)).Select(x => (x.Id.Value, x.NombreGenerico, x.MgPorUnidad, x.UnidadesPorToma)).ToList();
    }
}
