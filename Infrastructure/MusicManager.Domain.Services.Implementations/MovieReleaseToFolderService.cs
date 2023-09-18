using IWshRuntimeLibrary;
using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
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

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(MovieRelease movieRelease, Movie parent)
    {
        if (parent.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error("Parent directory info is not created."));
        }

        var rootDirectory = new DirectoryInfo(_root.CombineWith(parent.EntityDirectoryInfo.Path));
        if (!rootDirectory.Exists)
        {
            return Result.Failure<string>(new Error("Parent directory is not exists."));
        }

        string createdMovieReleaseDirectoryName = GetDirectoryName(movieRelease);
        string createdMovieReleaseDirectoryFullPath = Path.Combine(rootDirectory.FullName, createdMovieReleaseDirectoryName);
        string createdMovieReleaseRelationalPath = createdMovieReleaseDirectoryFullPath.GetRelational(_root);

        if (Directory.Exists(createdMovieReleaseDirectoryFullPath)
            || await _dbContext.MovieReleases.AnyAsync(e => e.EntityDirectoryInfo == EntityDirectoryInfo.Create(createdMovieReleaseRelationalPath).Value))
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

    public Task<Result> CreateFolderLinkAsync(Movie addLinkTo, string movieReleaseLinkTargetPath)
    {
        if (addLinkTo.EntityDirectoryInfo is null)
        {
            return Task.FromResult(Result.Failure(new Error("Movie directory info is not created. Can't add link to this movie.")));
        }

        string targetDirectory = _root.CombineWith(movieReleaseLinkTargetPath);
        string shortcutStorageDirectory = _root.CombineWith(addLinkTo.EntityDirectoryInfo.Path);

        if (!Directory.Exists(shortcutStorageDirectory))
        {
            return Task.FromResult(Result.Failure(new Error("Movie directory is not exists.")));
        }

        if (!Directory.Exists(shortcutStorageDirectory))
        {
            return Task.FromResult(Result.Failure(new Error("Target movie release directory is not exists.")));
        }
        
        string shorcutName = $"{Path.GetFileName(targetDirectory)} - shortcut.lnk";
        string shorcutFullPath = Path.Combine(shortcutStorageDirectory, shorcutName);

        WshShell shell = new();
        WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(shorcutFullPath);
        shortcut.TargetPath = targetDirectory;
        shortcut.Save();

        return Task.FromResult(Result.Success());
    }

    public async Task<Result<string>> UpdateIfExistsAsync(MovieRelease movieRelease)
    {
        if (movieRelease.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error("Movie release directory info is not created."));
        }

        var previousDirectory = new DirectoryInfo(_root.CombineWith(movieRelease.EntityDirectoryInfo!.Path));
        string previousDirectoryName = previousDirectory.Name;
        if (!previousDirectory.Exists)
        {
            return Result.Failure<string>(new Error("Original directory is not exists."));
        }

        string newDirectoryName = GetDirectoryName(movieRelease);
        string newDirectoryFullPath = Path.Combine(previousDirectory.Parent!.FullName, newDirectoryName);

        if (previousDirectory.FullName != newDirectoryFullPath)
        {
            previousDirectory.MoveTo(newDirectoryFullPath);
        }

        await movieRelease
            .ToJson()
            .AddSerializedJsonEntityToAsync(Path.Combine(newDirectoryFullPath, MovieReleaseEntityJson.FileName));

        await UpdateAllExistingLinks(movieRelease.Movies, previousDirectoryName, newDirectoryFullPath);
        return Result.Success(newDirectoryFullPath.GetRelational(_root));
    }

    private async Task UpdateAllExistingLinks(IEnumerable<Movie> movies, string previousDirectoryName, string newDirectoryFullPath)
    {
        var moviesToUpdateLinksIn = movies.Where(e => e.EntityDirectoryInfo is not null);

        foreach (var item in moviesToUpdateLinksIn)
        {
            string linkStorageDirectory = _root.CombineWith(item.EntityDirectoryInfo!.Path);

            var linkInfo = new DirectoryInfo(Path.Combine(linkStorageDirectory, previousDirectoryName));
            if (linkInfo.Exists && linkInfo.LinkTarget is not null)
            {
                linkInfo.Delete();
                await CreateFolderLinkAsync(item, newDirectoryFullPath);
                continue;
            }

            var shortcutInfo = new DirectoryInfo(linkStorageDirectory)
                .EnumerateFiles()
                .FirstOrDefault(e => e.Name.Contains(previousDirectoryName) && e.Extension == ".lnk");
            if (shortcutInfo is not null)
            {
                shortcutInfo.Delete();
                await CreateFolderLinkAsync(item, newDirectoryFullPath);
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

        string createdMovieReleaseDirectoryName = $"{baseMovieReleaseDirectoryName} {DomainServicesConstants.DiscDirectoryNameSeparator} {movieRelease.ProductionInfo?.Country ?? ProductionInfo.UndefinedCountry} " +
        $"{DomainServicesConstants.DiscDirectoryNameSeparator} {movieRelease.ProductionInfo!.Year}";

        return createdMovieReleaseDirectoryName;
    }
}
