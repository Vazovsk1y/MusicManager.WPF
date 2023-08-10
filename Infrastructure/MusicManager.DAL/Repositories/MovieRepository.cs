using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Models;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IApplicationDbContext _dbContext;

    public MovieRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Movie>> LoadAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Movies
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Movie>> LoadAllWithMovieReleasesAsync(IEnumerable<MovieId> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Movies.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Movie?> LoadByIdAsync(MovieId id, CancellationToken cancellationToken = default)
        => await _dbContext.Movies
        .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Movie?> LoadByIdWithMoviesReleasesAsync(MovieId id, CancellationToken cancellation = default)
        => await _dbContext.Movies
        .Include(e => e.Releases)
        .ThenInclude(e => e.Movies)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);
}
