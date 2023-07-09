using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface ISongwriterService
    {
        Task<Result> SaveFromFolderAsync(ISongwriterFolder songwriterFolder, CancellationToken cancellationToken = default);
    }
}