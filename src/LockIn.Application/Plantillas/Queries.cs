// File: src/LockIn.Application/Plantillas/Queries.cs
namespace LockIn.Application.Plantillas;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using LockIn.Domain.Repositories;
using LockIn.Application.DTOs;
using LockIn.Application.Common;
using LockIn.Domain.ValueObjects;

public static class Queries
{
    public sealed record GetTemplateById(Guid PlantillaId) : IRequest<PlantillaDto?>;
    public sealed class GetTemplateByIdHandler : IRequestHandler<GetTemplateById, PlantillaDto?>
    {
        private readonly IPlantillasRepo _repo;
        public GetTemplateByIdHandler(IPlantillasRepo repo) => _repo = repo;
        public async Task<PlantillaDto?> Handle(GetTemplateById r, CancellationToken ct)
            => (await _repo.GetAsync(new PlantillaId(r.PlantillaId), ct))?.ToDto();
    }

    public sealed record ListTemplates(bool? SoloConfirmadas) : IRequest<IReadOnlyCollection<PlantillaDto>>;
    public sealed class ListTemplatesHandler : IRequestHandler<ListTemplates, IReadOnlyCollection<PlantillaDto>>
    {
        private readonly IPlantillasRepo _repo;
        public ListTemplatesHandler(IPlantillasRepo repo) => _repo = repo;
        public async Task<IReadOnlyCollection<PlantillaDto>> Handle(ListTemplates r, CancellationToken ct)
            => (await _repo.ListAsync(r.SoloConfirmadas, ct)).Select(t => t.ToDto()).ToList();
    }
}
