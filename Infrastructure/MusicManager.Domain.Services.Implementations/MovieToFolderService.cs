using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories.Data;
using MusicManager.Utils;

namespace MusicManager.Domain.Services.Implementations;

public class MovieToFolderService : IMovieToFolderService
{
    private readonly IApplicationDbContext _dbContext;

    public MovieToFolderService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(Movie movie, Songwriter parent)
    {
        if (parent.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error("Parent directory info is not created."));
        }

        var rootPath = Path.Combine(parent.EntityDirectoryInfo.FullPath, DomainServicesConstants.MOVIES_FOLDER_NAME);
        if (!Directory.Exists(rootPath))
        {
            return Result.Failure<string>(new Error("Parent directory is not exists."));
        }

        string createdMovieDirectoryName = $"{movie.ProductionInfo?.Year} {DomainServicesConstants.MovieDirectoryNameSeparator} {movie.Title}";
        string createdMovieDirectoryFullPath = Path.Combine(rootPath, createdMovieDirectoryName);

        if (Directory.Exists(createdMovieDirectoryFullPath)
            || await _dbContext.Movies.AnyAsync(e => e.EntityDirectoryInfo == EntityDirectoryInfo.Create(createdMovieDirectoryFullPath).Value))
        {
            return Result.Failure<string>(new Error("Directory for this movie is already exists or movie with that directory info is already added to database."));
        }

        DirectoryHelper.CreateIfNotExists(createdMovieDirectoryFullPath);

        string jsonFileInfoPath = Path.Combine(createdMovieDirectoryFullPath, MovieEntityJson.FileName);
        await movie
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return createdMovieDirectoryFullPath;
    }
}