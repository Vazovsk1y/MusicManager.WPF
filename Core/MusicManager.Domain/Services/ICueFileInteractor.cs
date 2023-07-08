using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface ICueFileInteractor 
{
    Task<Result<IEnumerable<ICueFileTrack>>> GetTracksAsync(string cueFilePath, CancellationToken cancellationToken = default);
}
