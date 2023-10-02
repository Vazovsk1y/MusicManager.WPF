using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IFolderToSongwriterService
{
    Task<Result<Songwriter>> GetEntityAsync(string songwriterPath);
}
