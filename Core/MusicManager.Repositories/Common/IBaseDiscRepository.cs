using MusicManager.Domain.Common;

namespace MusicManager.Repositories.Common;

public interface IBaseDiscRepository<T> : IRepository<T> where T : Disc
{
    Task<T?> GetByIdAsync(DiscId id, CancellationToken cancellationToken = default);

    Task<T?> GetByIdWithSongsAsync(DiscId id, CancellationToken cancellation = default);

    Task<T?> GetByIdWithCoversAsync(DiscId id, CancellationToken cancellation = default);

    Task<T?> GetByIdWithSongsAndCoversAsync(DiscId id, CancellationToken cancellation = default);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
}
