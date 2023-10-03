using IWshRuntimeLibrary;
using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Constants;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Storage;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories.Data;
using MusicManager.Utils;

namespace MusicManager.Domain.Services.Implementations;

public class MovieReleaseToFolderService : IMovieReleaseToFolderService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRoot _root;

    public MovieReleaseToFolderService(
        IApplicationDbContext dbContext, 
        IRoot root)
    {
        _dbContext = dbContext;
        _root = root;
    }

	public Task<Result<string>> CreateFolderLinkAsync(MovieRelease movieRelease, Movie movie, CancellationToken cancellationToken = default)
    {
		if (movie.AssociatedFolderInfo is null)
		{
			return Task.FromResult(Result.Failure<string>(new Error("Movie directory info is not created. Can't add link to this movie.")));
		}

        if (movieRelease.AssociatedFolderInfo is null)
        {
			return Task.FromResult(Result.Failure<string>(new Error("Movie release directory info is not created. Can't add this mr to movie.")));
		}

		string originalMovieReleaseFolder = _root.CombineWith(movieRelease.AssociatedFolderInfo.Path);
		string shortcutStorageFolder = _root.CombineWith(movie.AssociatedFolderInfo.Path);

		if (!Directory.Exists(shortcutStorageFolder))
		{
			return Task.FromResult(Result.Failure<string>(new Error("Movie directory is not exists.")));
		}

		if (!Directory.Exists(originalMovieReleaseFolder))
		{
			return Task.FromResult(Result.Failure<string>(new Error("Target movie release directory is not exists.")));
		}

		string shorcutName = $"{Path.GetFileName(originalMovieReleaseFolder)} - shortcut{DomainConstants.LnkExtension}";
		string shorcutFullPath = Path.Combine(shortcutStorageFolder, shorcutName);

        CreateShortcutFile(originalMovieReleaseFolder, shorcutFullPath);
		return Task.FromResult(Result.Success(shorcutFullPath.GetRelational(_root)));
	}

    private static void CreateShortcutFile(string targetPath, string shorcutPath)
    {
		WshShell shell = new();
		WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(shorcutPath);
		shortcut.TargetPath = targetPath;
		shortcut.Save();
	}

	public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(MovieRelease movieRelease, Movie parent, CancellationToken cancellationToken = default)
    {
        if (parent.AssociatedFolderInfo is null)
        {
            return Result.Failure<string>(new Error("Parent directory info is not created."));
        }

        var rootDirectory = new DirectoryInfo(_root.CombineWith(parent.AssociatedFolderInfo.Path));
        if (!rootDirectory.Exists)
        {
            return Result.Failure<string>(new Error("Parent directory is not exists."));
        }

        string createdMovieReleaseDirectoryName = GetDirectoryName(movieRelease);
        string createdMovieReleaseDirectoryFullPath = Path.Combine(rootDirectory.FullName, createdMovieReleaseDirectoryName);
        string createdMovieReleaseRelationalPath = createdMovieReleaseDirectoryFullPath.GetRelational(_root);

        if (Directory.Exists(createdMovieReleaseDirectoryFullPath)
            || await _dbContext.MovieReleases.AnyAsync(e => e.AssociatedFolderInfo == EntityDirectoryInfo.Create(createdMovieReleaseRelationalPath).Value))
        {
            return Result.Failure<string>(new Error("Directory for this movie release is already exists or movie release with that directory info is already added to database."));
        }

        var createdMovieReleaseDirectoryInfo = DirectoryHelper.CreateIfNotExists(createdMovieReleaseDirectoryFullPath);
        createdMovieReleaseDirectoryInfo.CreateSubdirectory(DomainServicesConstants.COVERS_FOLDER_NAME);

        string jsonFileInfoPath = Path.Combine(createdMovieReleaseDirectoryFullPath, MovieReleaseEntityJson.FileName);
        await movieRelease
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return createdMovieReleaseRelationalPath;
    }

    

    public async Task<Result<string>> UpdateAsync(MovieRelease movieRelease, CancellationToken cancellationToken = default)
    {
        if (movieRelease.AssociatedFolderInfo is null)
        {
            return Result.Failure<string>(new Error("Movie release directory info is not created."));
        }

		var moviesWhereStoresActualFolders = movieRelease.MoviesLinks.Where(item =>
		item.MovieReleaseId == movieRelease.Id
		&& item.ReleaseLinkInfo is null
		&& item.Movie.AssociatedFolderInfo is not null).Select(e => e.Movie);

        if (!moviesWhereStoresActualFolders.Any())
        {
            return Result.Failure<string>(new Error("Movie with original movieRelease folder not found."));
        }

		InvalidOperationExceptionHelper.ThrowIfTrue(moviesWhereStoresActualFolders.Count() > 1, "Detected more than one actual folder created.");

        string previousFolderFullPath = _root.CombineWith(movieRelease.AssociatedFolderInfo!.Path);
        string previousFolderName = Path.GetFileName(previousFolderFullPath);
        if (!Directory.Exists(previousFolderFullPath))
        {
            return Result.Failure<string>(new Error("Original directory is not exists."));
        }

        string newDirectoryName = GetDirectoryName(movieRelease);
        var movieWhereOriginalMovieReleaseFolderStores = moviesWhereStoresActualFolders.First();
		string newDirectoryFullPath = Path.Combine(_root.CombineWith(movieWhereOriginalMovieReleaseFolderStores.AssociatedFolderInfo!.Path), newDirectoryName);
        if (previousFolderFullPath != newDirectoryFullPath)
        {
            Directory.Move(previousFolderFullPath, newDirectoryFullPath);
        }

        await movieRelease
            .ToJson()
            .AddSerializedJsonEntityToAsync(Path.Combine(newDirectoryFullPath, MovieReleaseEntityJson.FileName));

        var moviesWhereStoresLinks = movieRelease.MoviesLinks.Select(e => e.Movie).ToList();
        moviesWhereStoresLinks.Remove(movieWhereOriginalMovieReleaseFolderStores);

		await UpdateAllExistingLinks(moviesWhereStoresLinks, previousFolderName, newDirectoryFullPath);
        return Result.Success(newDirectoryFullPath.GetRelational(_root));
    }

    private async Task UpdateAllExistingLinks(IEnumerable<Movie> movies, string previousDirectoryName, string newDirectoryFullPath)
    {
        var moviesToUpdateLinksIn = movies
            .Where(e => e.ReleasesLinks.Any(item => item.ReleaseLinkInfo is not null));

        foreach (var item in moviesToUpdateLinksIn)
        {
            string linkStorageDirectory = _root.CombineWith(item.AssociatedFolderInfo!.Path);

            var linkInfo = new DirectoryInfo(Path.Combine(linkStorageDirectory, previousDirectoryName));
            if (linkInfo.Exists && linkInfo.LinkTarget is not null)
            {
                linkInfo.Delete();
				string shorcutName = $"{Path.GetFileName(newDirectoryFullPath)} - shortcut{DomainConstants.LnkExtension}";
				string shorcutFullPath = Path.Combine(linkStorageDirectory, shorcutName);
				CreateShortcutFile(newDirectoryFullPath, shorcutFullPath);
                continue;
            }

            var shortcutInfo = new DirectoryInfo(linkStorageDirectory)
                .EnumerateFiles()
                .FirstOrDefault(e => e.Name.Contains(previousDirectoryName) && e.Extension == DomainConstants.LnkExtension);
            if (shortcutInfo is not null)
            {
                shortcutInfo.Delete();
				string shorcutName = $"{Path.GetFileName(newDirectoryFullPath)} - shortcut{DomainConstants.LnkExtension}";
				string shorcutFullPath = Path.Combine(linkStorageDirectory, shorcutName);
				CreateShortcutFile(newDirectoryFullPath, shorcutFullPath);
			}
        }
    }

    private string GetDirectoryName(MovieRelease movieRelease)
    {
        string baseMovieReleaseDirectoryName = $"{movieRelease.Type.Value} {movieRelease.Identifier}";
        if (movieRelease.Type == DiscType.Bootleg || movieRelease.Type == DiscType.Unknown)
        {
            return baseMovieReleaseDirectoryName;
        }

        string createdMovieReleaseDirectoryName = $"{baseMovieReleaseDirectoryName} {DomainServicesConstants.DiscFolderNameSeparator} {movieRelease.ProductionInfo?.Country ?? ProductionInfo.UndefinedCountry} " +
        $"{DomainServicesConstants.DiscFolderNameSeparator} {movieRelease.ProductionInfo!.Year}";

        return createdMovieReleaseDirectoryName;
    }
}
