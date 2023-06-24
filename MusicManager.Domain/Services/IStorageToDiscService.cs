using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IStorageToDiscService
{
    Task<Result<Disc>> GetEntityAsync(IStorage discStorage, Movie parentMovie);

    Task<Result<Disc>> GetEntityAsync(IStorage discStorage, SongwriterId songwriterId);
}
