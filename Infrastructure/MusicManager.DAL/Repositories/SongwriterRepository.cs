using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL.Repositories;

public class SongwriterRepository : ISongwriterRepository
{
    private readonly IApplicationDbContext _dbContext;

    public SongwriterRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> DeleteAsync(SongwriterId songwriterId, CancellationToken cancellationToken = default)
    {
        var entityToDelete = await _dbContext.Songwriters.SingleOrDefaultAsync(e => e.Id == songwriterId, cancellationToken);
        if (entityToDelete is not null)
        {
            _dbContext.Songwriters.Remove(entityToDelete);
            return true;
        }
        return false;
    }

    public async Task<IEnumerable<Songwriter>> GetAllAsync(CancellationToken cancellationToken = default) 
        => await _dbContext.Songwriters
        .ToListAsync(cancellationToken);

    public async Task<Songwriter?> GetByIdAsync(SongwriterId id, CancellationToken cancellationToken = default)
        => await _dbContext.Songwriters
        .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Songwriter?> GetByIdWithCompilationsAsync(SongwriterId id, CancellationToken cancellation = default) 
        => await _dbContext.Songwriters
        .Include(e => e.Compilations)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);

    public async Task<Songwriter?> GetByIdWithMoviesAndCompilationsAsync(SongwriterId id, CancellationToken cancellation = default)
        => await _dbContext.Songwriters
        .Include(e => e.Compilations)
        .Include(e => e.Movies)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);

    public async Task<Songwriter?> GetByIdWithMoviesAsync(SongwriterId id, CancellationToken cancellation = default)
        => await _dbContext.Songwriters
        .Include(e => e.Movies)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);

    public async Task<bool> InsertAsync(Songwriter songwriter, CancellationToken cancellationToken = default) 
        => await _dbContext.Songwriters.AddAsync(songwriter, cancellationToken) is not null;

    public async Task<bool> IsExistsWithPassedDirectoryInfo(EntityDirectoryInfo directoryInfo, CancellationToken cancellationToken = default) 
        => await _dbContext.Songwriters.AnyAsync(e => e.EntityDirectoryInfo == directoryInfo, cancellationToken);
}
