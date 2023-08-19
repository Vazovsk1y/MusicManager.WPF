using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Services;

public interface ISongToFileService
{
    Task<Result<string>> CopyToAsync(string songFileFullPath, Disc parent, DiscNumber? discNumber = null);
}
