using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IFolderToCompilationService
{
    Task<Result<Compilation>> GetEntityAsync(string compilationPath, SongwriterId songwriterId);
}
