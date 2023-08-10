using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL.Repositories;

public class MovieReleaseRepository : BaseDiscRepository<MovieRelease>, IMovieReleaseRepository
{
    public MovieReleaseRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<MovieRelease?> LoadWithMoviesAsync(DiscId id, CancellationToken cancellation = default)
        => await _dbContext.MovieReleases
        .Include(e => e.Movies)
        .ThenInclude(e => e.Releases)
        .SingleOrDefaultAsync(e => e.Id == id, cancellation);
}
