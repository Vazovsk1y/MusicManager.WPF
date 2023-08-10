using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface ICueFileInteractor 
{
    Task<Result<CueSheetInfo>> GetCueSheetAsync(string cueFilePath, CancellationToken cancellationToken = default);
}
