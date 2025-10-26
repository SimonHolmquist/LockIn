// File: src/LockIn.Application/Catalogos/KeyActivities.cs
namespace LockIn.Application.Catalogos;

using FluentValidation;
using LockIn.Domain.Entities;
using LockIn.Domain.Repositories;
using LockIn.Domain.ValueObjects;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class KeyActivities
{
    public sealed record CreateKeyActivity(string Nombre) : IRequest<Unit>;
    public sealed class CreateKeyActivityValidator : AbstractValidator<CreateKeyActivity>
    {
        public CreateKeyActivityValidator() => RuleFor(x => x.Nombre).NotEmpty();
    }
    public sealed class CreateKeyActivityHandler : IRequestHandler<CreateKeyActivity, Unit>
    {
        private readonly IKeyActivitiesRepo _repo; private readonly IUnitOfWork _uow;
        public CreateKeyActivityHandler(IKeyActivitiesRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<Unit> Handle(CreateKeyActivity r, CancellationToken ct)
        {
            await _repo.AddAsync(new KeyActivityDefinition(KeyActivityId.New(), r.Nombre), ct);
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }

    public sealed record DeleteKeyActivity(Guid Id) : IRequest<Unit>;
    public sealed class DeleteKeyActivityHandler : IRequestHandler<DeleteKeyActivity, Unit>
    {
        private readonly IKeyActivitiesRepo _repo; private readonly IUnitOfWork _uow;
        public DeleteKeyActivityHandler(IKeyActivitiesRepo repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
        public async Task<Unit> Handle(DeleteKeyActivity r, CancellationToken ct)
        {
            await _repo.DeleteAsync(new KeyActivityId(r.Id), ct);
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }

    public sealed record GetKeyActivities() : IRequest<IReadOnlyCollection<(Guid Id, string Nombre)>>;
    public sealed class GetKeyActivitiesHandler : IRequestHandler<GetKeyActivities, IReadOnlyCollection<(Guid, string)>>
    {
        private readonly IKeyActivitiesRepo _repo;
        public GetKeyActivitiesHandler(IKeyActivitiesRepo repo) => _repo = repo;
        public async Task<IReadOnlyCollection<(Guid, string)>> Handle(GetKeyActivities r, CancellationToken ct)
            => (await _repo.ListAsync(ct)).Select(x => (x.Id.Value, x.Nombre)).ToList();
    }
}
