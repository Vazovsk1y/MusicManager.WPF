using MusicManager.Domain.Common;

namespace MusicManager.Repositories.Common;

public interface IBaseDiscRepository<T> : IRepository<T> where T : Disc
{
    Task<T?> LoadByIdAsync(DiscId id, CancellationToken cancellationToken = default);

    Task<T?> LoadByIdWithSongsAsync(DiscId id, CancellationToken cancellation = default);

    Task<T?> LoadByIdWithCoversAsync(DiscId id, CancellationToken cancellation = default);

    Task<T?> LoadByIdWithSongsAndCoversAsync(DiscId id, CancellationToken cancellation = default);

    Task<IEnumerable<T>> LoadAllAsync(CancellationToken cancellationToken = default);
}
