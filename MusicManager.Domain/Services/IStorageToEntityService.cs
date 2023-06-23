using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services
{
    public interface IStorageToEntityService<T> where T : Entity
    {
        Task<Result<T>> GetEntityAsync(IStorage storage);
    }
}
