using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Common;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services.Implementations;

internal class BaseDiscService : IBaseDiscService
{
    private readonly IBaseDiscRepository<Disc> _discRepository;

    public BaseDiscService(IBaseDiscRepository<Disc> discRepository)
    {
        _discRepository = discRepository;
    }

    public async Task<Result<IEnumerable<DiscLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        var discs = await _discRepository.LoadAllAsync(cancellationToken);
        return discs.Select(e => new DiscLookupDTO(e.Id, e.Identifier)).ToList();
    }
}
