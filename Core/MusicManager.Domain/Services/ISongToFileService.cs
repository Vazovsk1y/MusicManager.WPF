using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Services;

public interface ISongToFileService
{
    Task<Result<string>> CopyToAsync(string songFilePath, Disc parent, DiscNumber? discNumber = null);

    Task<Result<string>> UpdateIfExistsAsync(Song song, CancellationToken cancellationToken = default);
}
