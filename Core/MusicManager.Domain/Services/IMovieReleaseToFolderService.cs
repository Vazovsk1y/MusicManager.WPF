using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IMovieReleaseToFolderService
{
    Task<Result<string>> CreateAssociatedFolderAndFileAsync(MovieRelease movieRelease, Movie parent, CancellationToken cancellationToken = default);

    Task<Result<string>> CreateFolderLinkAsync(MovieRelease movieRelease, Movie movie, CancellationToken cancellationToken = default);

    Task<Result<string>> UpdateAsync(MovieRelease movieRelease, CancellationToken cancellationToken = default);
}
