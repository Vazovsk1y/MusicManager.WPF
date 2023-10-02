using IWshRuntimeLibrary;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Services;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Factories;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MusicManager.Services.Implementations.Contracts.Factories;

public class MovieFolderFactory : IMovieFolderFactory
{
    private readonly IDiscFolderFactory _movieReleaseFolderFactory;
    private readonly IRoot _root;
    public MovieFolderFactory(IDiscFolderFactory movieReleaseFolderFactory, IRoot root)
    {
        _movieReleaseFolderFactory = movieReleaseFolderFactory;
        _root = root;
    }

    public Result<MovieFolder> Create(DirectoryInfo movieDirectory)
    {
        if (!movieDirectory.Exists)
        {
            return Result.Failure<MovieFolder>(DomainServicesErrors.PassedDirectoryIsNotExists(movieDirectory.FullName));
        }

        List<DiscFolder> movieReleases = new();
        var moviesReleasesDirectories = new List<(DirectoryInfo actualDirectory, string? linkOnActualDirectory)>();

        // if shortcut file with .lnk extension, if link directory with not null link target.
        var fileSystemInfos = movieDirectory.EnumerateFileSystemInfos().Where(e => e.LinkTarget is not null || e.Extension == ".lnk");  

        foreach (var fileSystemInfo in fileSystemInfos)
        {
            if (fileSystemInfo.Extension == ".lnk")
            {
                WshShell shell = new();
                WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(fileSystemInfo.FullName);
                if (shortcut.TargetPath.Contains(DomainServicesConstants.COMPILATIONS_FOLDER_NAME))
                {
                    // asked to skip, he will delete it by himself
                    continue;
                }

                var shorcut = Shortcut(fileSystemInfo, shortcut);
                if (shorcut.IsFailure)
                {
                    return Result.Failure<MovieFolder>(shorcut.Error);
                }

                moviesReleasesDirectories.Add((shorcut.Value, fileSystemInfo.FullName));
            }
            else
            {
                moviesReleasesDirectories.Add((new DirectoryInfo(fileSystemInfo.LinkTarget!), fileSystemInfo.FullName));
            }
        }

        moviesReleasesDirectories.AddRange(movieDirectory.EnumerateDirectories().Where(e => e.LinkTarget is null).Select(e => (e, (string?)null)));  // exclude already added directories links

        foreach (var movieReleaseDirectory in moviesReleasesDirectories)
        {
            var result = _movieReleaseFolderFactory.Create(movieReleaseDirectory.actualDirectory, movieReleaseDirectory.linkOnActualDirectory);
            if (result.IsFailure)
            {
                return Result.Failure<MovieFolder>(result.Error);
            }

            movieReleases.Add(result.Value);
        }

        return new MovieFolder(movieDirectory.FullName, movieReleases);
    }

    private Result<DirectoryInfo> Shortcut(FileSystemInfo currentShortcut, WshShortcut actualShorcut)
    {
        if (!Directory.Exists(actualShorcut.TargetPath))
        {
            // three level back  {SomeCompositor}/movies/{SomeMovie}/{CurrentShorcutFile}

            string pathAfterMoviesWord = actualShorcut.TargetPath[actualShorcut.TargetPath.IndexOf(DomainServicesConstants.MOVIES_FOLDER_NAME)..];
            string pathBeforeMoviesFolder =
                actualShorcut.TargetPath[..(actualShorcut.TargetPath.Length - pathAfterMoviesWord.Length - 1)];

            var songwriterBuilder = new StringBuilder();
            foreach (var item in pathBeforeMoviesFolder.Reverse())
            {
                if (item == '\\')
                {
                    break;
                }

                songwriterBuilder.Append(item);
            }
            
            string songwriterName = new(songwriterBuilder.ToString().Reverse().ToArray());
            string newTargetPath = Path.Combine(
                _root.CombineWith(songwriterName),
                pathAfterMoviesWord
                );

            if (!Directory.Exists(newTargetPath))
            {
                return Result.Failure<DirectoryInfo>(new Error($"The target path of lnk file {currentShortcut.Name} is not exists\n." +
                    $"Target path {newTargetPath}."));
            }

            actualShorcut.TargetPath = newTargetPath;
            actualShorcut.Save();
            return new DirectoryInfo(newTargetPath);
        }

        return new DirectoryInfo(actualShorcut.TargetPath);
    }
}

