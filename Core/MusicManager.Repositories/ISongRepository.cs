using MusicManager.Domain.Models;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface ISongRepository : IRepository<Song>
{
    Task<Song?> GetByIdAsync(SongId id, CancellationToken cancellationToken = default);

    Task<Song?> GetByIdWithPlaybackInfoAsync(SongId id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Song>> GetAllAsync();
}
