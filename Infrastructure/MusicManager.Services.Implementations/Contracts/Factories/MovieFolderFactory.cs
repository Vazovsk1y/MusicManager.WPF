using IWshRuntimeLibrary;
using MusicManager.Domain.Constants;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Services;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Factories;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusicManager.Services.Implementations.Contracts.Factories;

public class MovieFolderFactory : IMovieFolderFactory
{
    private readonly IDiscFolderFactory _movieReleaseFolderFactory;
    private readonly IRoot _root;
    public MovieFolderFactory(
        IDiscFolderFactory movieReleaseFolderFactory, 
        IRoot root)
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

        var moviesReleasesResult = GetMoviesReleases(movieDirectory);
        if (moviesReleasesResult.IsFailure)
        {
            return Result.Failure<MovieFolder>(moviesReleasesResult.Error);
        }

		return new MovieFolder(movieDirectory.FullName, moviesReleasesResult.Value);
    }

    private Result<IReadOnlyCollection<DiscFolder>> GetMoviesReleases(DirectoryInfo movieDirectory)
    {
		List<DiscFolder> movieReleasesFolders = new();
		foreach (var fileSystemInfo in movieDirectory.EnumerateFileSystemInfos())
		{
			(DirectoryInfo actualMovieReleaseDirectory, string? linkOnThisDirectory) currentMovieReleaseDirectory = (null, null);
			if (string.Equals(fileSystemInfo.Extension, DomainConstants.LnkExtension, StringComparison.OrdinalIgnoreCase))
			{
				WshShell shell = new();
				WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(fileSystemInfo.FullName);
				if (shortcut.TargetPath.Contains(DomainServicesConstants.COMPILATIONS_FOLDER_NAME))
				{
					// asked to skip, he will delete it by himself
					continue;
				}

				var targetShorcutFolderResult = GetTarget(fileSystemInfo, shortcut);
				if (targetShorcutFolderResult.IsFailure)
				{
					return Result.Failure<IReadOnlyCollection<DiscFolder>>(targetShorcutFolderResult.Error);
				}

				currentMovieReleaseDirectory = (targetShorcutFolderResult.Value, fileSystemInfo.FullName);
			}
			else if (fileSystemInfo is DirectoryInfo { LinkTarget: { } })
			{
				currentMovieReleaseDirectory = (new DirectoryInfo(fileSystemInfo.LinkTarget!), fileSystemInfo.FullName);
			}
			else if (fileSystemInfo is DirectoryInfo directoryInfo)
			{
				currentMovieReleaseDirectory = (directoryInfo, null);
			}

            if (currentMovieReleaseDirectory is { actualMovieReleaseDirectory: {} })
            {
				var result = _movieReleaseFolderFactory.Create(currentMovieReleaseDirectory.actualMovieReleaseDirectory, currentMovieReleaseDirectory.linkOnThisDirectory);
				if (result.IsFailure)
				{
					return Result.Failure<IReadOnlyCollection<DiscFolder>>(result.Error);
				}

				movieReleasesFolders.Add(result.Value);
			}
		}

        return movieReleasesFolders;
	}

    private Result<DirectoryInfo> GetTarget(FileSystemInfo currentShortcut, WshShortcut actualShorcut)
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
                return Result.Failure<DirectoryInfo>(new Error($"The expected target path of lnk file {currentShortcut.Name} is not exists\n." +
                    $"Expected target path {newTargetPath}."));
            }

            actualShorcut.TargetPath = newTargetPath;
            actualShorcut.Save();
            return new DirectoryInfo(newTargetPath);
        }

        return new DirectoryInfo(actualShorcut.TargetPath);
    }
}

