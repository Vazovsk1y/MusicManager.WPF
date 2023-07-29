using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Factories;
using System.IO;

namespace MusicManager.Services.Implementations.Contracts.Factories;

public class MovieFolderFactory : IMovieFolderFactory
{
    private readonly IDiscFolderFactory _movieReleaseFolderFactory;

    public MovieFolderFactory(IDiscFolderFactory movieReleaseFolderFactory)
    {
        _movieReleaseFolderFactory = movieReleaseFolderFactory;
    }

    public Result<MovieFolder> Create(DirectoryInfo movieDirectory)
    {
        if (!movieDirectory.Exists)
        {
            return Result.Failure<MovieFolder>(DomainServicesErrors.PassedDirectoryIsNotExists(movieDirectory.FullName));
        }

        List<DiscFolder> moviesReleasesFolders = new();
        var moviesReleasesDirectories = movieDirectory.EnumerateDirectories();

        foreach (var movieReleaseDirectory in moviesReleasesDirectories)
        {
            var result = _movieReleaseFolderFactory.Create(movieReleaseDirectory);
            if (result.IsFailure)
            {
                return Result.Failure<MovieFolder>(result.Error);
            }

            moviesReleasesFolders.Add(result.Value);
        }

        return new MovieFolder(movieDirectory.FullName, moviesReleasesFolders);
    }
}

