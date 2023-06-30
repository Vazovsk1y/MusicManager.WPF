using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IPathToSongService
{
    Task<Result<Song>> GetEntityAsync(string songPath, DiscId parentId);

    Task<Result<IEnumerable<Song>>> GetEntitiesFromCueFileAsync(string songFilePath, string cueFilePath, DiscId parentId);
}
