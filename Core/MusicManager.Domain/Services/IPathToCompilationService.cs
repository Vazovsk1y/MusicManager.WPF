using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IPathToCompilationService
{
    Task<Result<Compilation>> GetEntityAsync(string compilationPath, SongwriterId songwriterId);
}
