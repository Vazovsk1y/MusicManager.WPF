using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services;

public interface IBaseDiscService
{
    Task<Result<IEnumerable<DiscLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default);
}
