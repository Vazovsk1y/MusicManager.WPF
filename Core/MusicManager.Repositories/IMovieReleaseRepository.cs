using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface IMovieReleaseRepository : IDiscRepository<MovieRelease>
{
    Task<MovieRelease?> GetWithMoviesAsync(DiscId id, CancellationToken cancellation = default);
}
