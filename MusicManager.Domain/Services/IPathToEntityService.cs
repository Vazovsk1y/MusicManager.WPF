using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services
{
    public interface IPathToEntityService<T> where T : Entity
    {
        Task<Result<T>> GetEntityAsync(IStorage storage);

        Task<Result<IEnumerable<T>>> GetEntitiesAsync(IEnumerable<IStorage> storages);
    }
}
