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
    private readonly IRoot _root;

    public MovieToFolderService(IApplicationDbContext dbContext, IRoot root)
    {
        _dbContext = dbContext;
        _root = root;
    }

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(Movie movie, Songwriter parent)
    {
        if (parent.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error("Parent directory info is not created."));
        }

        var rootDirectory = new DirectoryInfo(Path.Combine(_root.CombineWith(parent.EntityDirectoryInfo.Path), DomainServicesConstants.MOVIES_FOLDER_NAME));
        if (!rootDirectory.Exists)
        {
            return Result.Failure<string>(new Error("Parent directory is not exists."));
        }

        string createdMovieDirectoryName = $"{movie.ProductionInfo?.Year} {DomainServicesConstants.MovieDirectoryNameSeparator} {movie.Title}";
        string createdMovieDirectoryFullPath = Path.Combine(rootDirectory.FullName, createdMovieDirectoryName);
        string createdMovieRelationalPath = createdMovieDirectoryFullPath.GetRelational(_root);

        if (Directory.Exists(createdMovieDirectoryFullPath)
            || await _dbContext.Movies.AnyAsync(e => e.EntityDirectoryInfo == EntityDirectoryInfo.Create(createdMovieRelationalPath).Value))
        {
            return Result.Failure<string>(new Error("Directory for this movie is already exists or movie with that directory info is already added to database."));
        }

        DirectoryHelper.CreateIfNotExists(createdMovieDirectoryFullPath);
        string jsonFileInfoPath = Path.Combine(createdMovieDirectoryFullPath, MovieEntityJson.FileName);
        await movie
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return createdMovieRelationalPath;
    }
}