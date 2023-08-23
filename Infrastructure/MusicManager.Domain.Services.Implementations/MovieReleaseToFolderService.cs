using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories.Data;
using MusicManager.Utils;
using System.Diagnostics;

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

        string baseMovieReleaseDirectoryName = $"{movieRelease.Type.Value} {movieRelease.Identifier}";
        string createdMovieReleaseDirectoryName = movieRelease.ProductionInfo is null || movieRelease.ProductionInfo.Year is null ?
        baseMovieReleaseDirectoryName
        :
        $"{baseMovieReleaseDirectoryName} {DomainServicesConstants.DiscDirectoryNameSeparator} {movieRelease.ProductionInfo.Country} " +
        $"{DomainServicesConstants.DiscDirectoryNameSeparator} {movieRelease.ProductionInfo.Year}";

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

    public async Task<Result> CreateFolderLinkAsync(Movie addLinkTo, string movieReleaseLinkTargetPath)
    {
        if (addLinkTo.EntityDirectoryInfo is null)
        {
            return Result.Failure(new Error("Movie directory info is not created. Can't add link to this movie."));
        }

        var toDirectory = new DirectoryInfo(_root.CombineWith(addLinkTo.EntityDirectoryInfo.Path));
        var fromDirectory = new DirectoryInfo(_root.CombineWith(movieReleaseLinkTargetPath));

        if (!toDirectory.Exists)
        {
            return Result.Failure(new Error("Movie directory is not exists."));
        }

        if (!fromDirectory.Exists)
        {
            return Result.Failure(new Error("Target movie release directory is not exists."));
        }

        string sourceDirectory = fromDirectory.FullName;
        string linkStorageDirectory = toDirectory.FullName;
        string linkName = Path.GetFileName(sourceDirectory);
        string linkPath = Path.Combine(linkStorageDirectory, linkName);
        string cmdCommand = $"/c mklink /D \"{linkPath}\" \"{sourceDirectory}\"";

        bool isLinkAlreadyExists = toDirectory.GetLink(linkName) is not null;
        if (isLinkAlreadyExists)
        {
            return Result.Failure(new Error("Link for this movie release is already exists."));
        }

        using var process = new Process
        {
            StartInfo = new()
            {
                FileName = "cmd.exe",
                Arguments = cmdCommand,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                WorkingDirectory = toDirectory.FullName
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            string messageWithError = await process.StandardError.ReadToEndAsync();
            return Result.Failure(new (messageWithError));
        }

        return Result.Success();
    }
}
