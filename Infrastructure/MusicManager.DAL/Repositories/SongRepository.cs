using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Models;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL.Repositories;

public class SongRepository : ISongRepository
{
    private readonly IApplicationDbContext _dbContext;

    public SongRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Song>> LoadAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Songs
        .ToListAsync(cancellationToken);

    public async Task<Song?> LoadByIdAsync(SongId id, CancellationToken cancellationToken = default)
        => await _dbContext.Songs
        .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Song?> LoadByIdWithPlaybackInfoAsync(SongId id, CancellationToken cancellationToken = default)
        => await _dbContext.Songs
        .Include(e => e.PlaybackInfo)
        .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
}
