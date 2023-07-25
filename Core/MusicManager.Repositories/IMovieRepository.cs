using MusicManager.Domain.Models;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    Task<Movie?> LoadByIdAsync(MovieId id, CancellationToken cancellationToken = default);

    Task<Movie?> LoadByIdWithMoviesReleasesAsync(MovieId id, CancellationToken cancellation = default);

    Task<IEnumerable<Movie>> LoadAllAsync(CancellationToken cancellationToken = default);
}
