using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Repositories.Common;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL.Repositories;

public abstract class BaseDiscRepository<TDisc> : IBaseDiscRepository<TDisc> where TDisc : Disc
{
    protected readonly IApplicationDbContext _dbContext;

    public BaseDiscRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TDisc>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Set<TDisc>()
        .ToListAsync();

    public async Task<TDisc?> GetByIdAsync(DiscId id, CancellationToken cancellationToken = default) 
        => await _dbContext.Set<TDisc>()
        .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<TDisc?> GetByIdWithCoversAsync(DiscId id, CancellationToken cancellation = default)
        => await _dbContext.Set<TDisc>()
        .Include(e => e.Covers)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);

    public async Task<TDisc?> GetByIdWithSongsAndCoversAsync(DiscId id, CancellationToken cancellation = default)
        => await _dbContext.Set<TDisc>()
        .Include(e => e.Covers)
        .Include(e => e.Songs)
        .ThenInclude(e => e.PlaybackInfo)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);

    public async Task<TDisc?> GetByIdWithSongsAsync(DiscId id, CancellationToken cancellation = default)
        => await _dbContext.Set<TDisc>()
        .Include(e => e.Songs)
        .ThenInclude(e => e.PlaybackInfo)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);
}
