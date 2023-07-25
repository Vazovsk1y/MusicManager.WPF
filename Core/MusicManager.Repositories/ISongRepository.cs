using MusicManager.Domain.Models;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface ISongRepository : IRepository<Song>
{
    Task<Song?> LoadByIdAsync(SongId id, CancellationToken cancellationToken = default);

    Task<Song?> LoadByIdWithPlaybackInfoAsync(SongId id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Song>> LoadAllAsync(CancellationToken cancellation = default);
}
