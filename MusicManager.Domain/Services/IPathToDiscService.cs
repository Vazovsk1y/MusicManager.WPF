using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IPathToDiscService
{
    Task<Result<Disc>> GetEntityAsync(string discPath, SongwriterId parentId);
}
