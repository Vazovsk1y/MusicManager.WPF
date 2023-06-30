using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IPathToSongwriterService
{
    Task<Result<Songwriter>> GetEntityAsync(string songwriterPath);
}
