using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface ISongwriterToFolderService
{
    Task<Result<string>> CreateAssociatedFolderAndFileAsync(Songwriter songwriter);
}
