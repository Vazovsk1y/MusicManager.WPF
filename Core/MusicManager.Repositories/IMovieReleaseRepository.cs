using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface IMovieReleaseRepository : IBaseDiscRepository<MovieRelease>
{
    Task<MovieRelease?> LoadWithMoviesAsync(DiscId id, CancellationToken cancellation = default);
}
