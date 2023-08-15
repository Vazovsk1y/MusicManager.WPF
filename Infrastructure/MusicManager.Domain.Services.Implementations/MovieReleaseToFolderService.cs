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

    public MovieReleaseToFolderService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(MovieRelease movieRelease, Movie parent)
    {
        if (parent.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error("Parent directory info is not created."));
        }

        var rootPath = parent.EntityDirectoryInfo.FullPath;
        if (!Directory.Exists(rootPath))
        {
            return Result.Failure<string>(new Error("Parent directory is not exists."));
        }

        string baseMovieReleaseDirectoryName = $"{movieRelease.Type.MapToString()} {movieRelease.Identifier}";
        string createdMovieReleaseDirectoryName = movieRelease.ProductionInfo is null ?
        baseMovieReleaseDirectoryName
        :
        $"{baseMovieReleaseDirectoryName} {DomainServicesConstants.DiscDirectoryNameSeparator} {movieRelease.ProductionInfo.Country} " +
        $"{DomainServicesConstants.DiscDirectoryNameSeparator} {movieRelease.ProductionInfo.Year}";

        string createdMovieReleaseDirectoryFullPath = Path.Combine(rootPath, createdMovieReleaseDirectoryName);
        if (Directory.Exists(createdMovieReleaseDirectoryFullPath)
            || await _dbContext.MovieReleases.AnyAsync(e => e.EntityDirectoryInfo == EntityDirectoryInfo.Create(createdMovieReleaseDirectoryFullPath).Value))
        {
            return Result.Failure<string>(new Error("Directory for this movie release is already exists or movie release with that directory info is already added to database."));
        }

        var createdMovieReleaseDirectoryInfo = DirectoryHelper.CreateIfNotExists(createdMovieReleaseDirectoryFullPath);
        createdMovieReleaseDirectoryInfo.CreateSubdirectory(DomainServicesConstants.COVERS_FOLDER_NAME);

        string jsonFileInfoPath = Path.Combine(createdMovieReleaseDirectoryFullPath, MovieReleaseEntityJson.FileName);
        await movieRelease
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return createdMovieReleaseDirectoryFullPath;
    }

    public async Task<Result> CreateFolderLinkAsync(Movie addLinkTo, string movieReleaseLinkTargetPath)
    {
        if (addLinkTo.EntityDirectoryInfo is null)
        {
            return Result.Failure(new Error("Movie directory info is not created. Link not added to this movie."));
        }

        var toDirectory = new DirectoryInfo(addLinkTo.EntityDirectoryInfo.FullPath);
        var fromDirectory = new DirectoryInfo(movieReleaseLinkTargetPath);

        if (!toDirectory.Exists)
        {
            return Result.Failure(new Error("Movie directory is not exists."));
        }

        if (!fromDirectory.Exists)
        {
            return Result.Failure(new Error("Movie release directory is not exists."));
        }

        string sourceDirectory = movieReleaseLinkTargetPath;
        string linkStorageDirectory = addLinkTo.EntityDirectoryInfo.FullPath;
        string linkName = Path.GetFileName(movieReleaseLinkTargetPath);
        string linkPath = Path.Combine(linkStorageDirectory, linkName);
        string cmdArgs = $"/c mklink /D \"{linkPath}\" \"{sourceDirectory}\"";

        bool isLinkAlreadyExists = fromDirectory.GetLink(linkName) is not null;
        if (isLinkAlreadyExists)
        {
            return Result.Failure(new Error("Link for this movie release is already exists."));
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = cmdArgs,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                WorkingDirectory = addLinkTo.EntityDirectoryInfo.FullPath
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            string messageWithError = await process.StandardError.ReadToEndAsync();
            return Result.Failure(new Error(messageWithError));
        }

        return Result.Success();
    }
}
