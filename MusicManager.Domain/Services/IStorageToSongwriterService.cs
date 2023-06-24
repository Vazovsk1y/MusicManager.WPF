using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IStorageToSongwriterService
{
    Task<Result<Songwriter>> GetEntityAsync(IStorage songwriterStorage);
}
