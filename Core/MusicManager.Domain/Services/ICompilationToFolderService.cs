using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface ICompilationToFolderService
{
    Task<Result<string>> CreateAssociatedFolderAndFileAsync(Compilation compilation, Songwriter parent, CancellationToken cancellationToken = default);

    Task<Result<string>> UpdateAsync(Compilation compilation, CancellationToken cancellationToken = default);
}