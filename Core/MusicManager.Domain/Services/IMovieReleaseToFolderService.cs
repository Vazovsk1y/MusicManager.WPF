using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface IMovieReleaseToFolderService
{
    Task<Result<string>> CreateAssociatedFolderAndFileAsync(MovieRelease movieRelease, Movie parent);

    Task<Result> CreateFolderLinkAsync(Movie addLinkTo, string movieReleaseLinkTargetPath);
}