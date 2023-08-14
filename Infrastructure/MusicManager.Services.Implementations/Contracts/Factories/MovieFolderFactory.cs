using IWshRuntimeLibrary;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Factories;
using System.IO;
using System.Windows.Forms;

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
        var moviesReleasesDirectories = new List<DirectoryInfo>();

        // if shortcut file with .lnk extension, if link directory with not null link target.
        var fileSystemInfos = movieDirectory.EnumerateFileSystemInfos().Where(e => e.LinkTarget is not null || e.Extension == ".lnk");  

        foreach (var fileSystemInfo in fileSystemInfos)
        {
            if (fileSystemInfo.Extension == ".lnk")
            {
                WshShell shell = new();
                WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(fileSystemInfo.FullName);
                moviesReleasesDirectories.Add(new DirectoryInfo(shortcut.TargetPath));
            }
            else
            {
                moviesReleasesDirectories.Add(new DirectoryInfo(fileSystemInfo.LinkTarget!));
            }
        }

        moviesReleasesDirectories.AddRange(movieDirectory.EnumerateDirectories().Where(e => e.LinkTarget is null));  // exclude already added directories links

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

