using IWshRuntimeLibrary;
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
        var moviesReleasesDirectories = new List<DirectoryInfo>(movieDirectory.EnumerateDirectories());
        var linksFiles = movieDirectory.EnumerateFiles().Where(e => e.Extension == ".lnk");

        foreach (var linkFile in linksFiles)
        {
            WshShell shell = new();
            WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(linkFile.FullName);
            moviesReleasesDirectories.Add(new DirectoryInfo(shortcut.TargetPath));
        }

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

