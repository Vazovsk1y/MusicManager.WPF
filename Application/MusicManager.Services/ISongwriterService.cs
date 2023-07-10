using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface ISongwriterService
    {
        Task<Result> SaveFromFolderAsync(SongwriterFolder songwriterFolder, CancellationToken cancellationToken = default);
    }
}