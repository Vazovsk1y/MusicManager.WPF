using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services.Implementations;

internal class BaseDiscService : IBaseDiscService
{
    private readonly IApplicationDbContext _dbContext;

    public BaseDiscService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<DiscLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Discs
            .AsNoTracking()
            .Select(e => new DiscLookupDTO(e.Id, e.Identifier))
            .ToListAsync(cancellationToken);
    }
}
