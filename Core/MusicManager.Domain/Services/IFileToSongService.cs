using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IFileToSongService
{
    Task<Result<Song>> GetEntityAsync(string executableSongFilePath, DiscId parentId);

    Task<Result<IEnumerable<Song>>> GetEntitiesFromCueFileAsync(string cueFilePath, DiscId parentId);
}
