using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface ISongwriterRepository : IRepository<Songwriter>
{
    Task<Songwriter?> GetByIdAsync(SongwriterId id, CancellationToken cancellationToken = default);

    Task<bool> IsExistsWithPassedDirectoryInfo(EntityDirectoryInfo directoryInfo, CancellationToken cancellationToken = default);

    Task<Songwriter?> GetByIdWithMoviesAsync(SongwriterId id, CancellationToken cancellation = default);

    Task<Songwriter?> GetByIdWithCompilationsAsync(SongwriterId id, CancellationToken cancellation = default);

    Task<Songwriter?> GetByIdWithMoviesAndCompilationsAsync(SongwriterId id, CancellationToken cancellation = default);

    Task<IEnumerable<Songwriter>> LoadAllAsync(CancellationToken cancellationToken = default);

    Task<bool> InsertAsync(Songwriter songwriter, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(SongwriterId songwriterId, CancellationToken cancellationToken = default);
}
