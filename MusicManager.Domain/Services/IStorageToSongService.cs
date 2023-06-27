using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IStorageToSongService
{
    Task<Result<Song>> GetEntityAsync(IStorage songStorage, DiscId parentId);

    Task<Result<IEnumerable<Song>>> GetEntitiesFromCueFileAsync(IStorage cueFilePath, DiscId parentId);
}
