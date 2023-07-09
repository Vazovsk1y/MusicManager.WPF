using MusicManager.Domain.Models;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    Task<Movie?> GetByIdAsync(MovieId id, CancellationToken cancellationToken = default);

    Task<Movie?> GetByIdWithMoviesReleasesAsync(MovieId id, CancellationToken cancellation = default);

    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default);
}
