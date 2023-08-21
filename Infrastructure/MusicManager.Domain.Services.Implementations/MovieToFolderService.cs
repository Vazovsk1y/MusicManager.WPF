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

    public async Task<Result<string>> CreateAssociatedAsync(Movie movie, Songwriter parent)
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

        string createdMovieDirectoryName = GetDirectoryName(movie);
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

    public async Task<Result<string>> UpdateIfExistsAsync(Movie movie)
    {
        if (movie.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error($"Associated folder isn't created."));
        }

        var currentDirectory = new DirectoryInfo(_root.CombineWith(movie.EntityDirectoryInfo.Path));
        if (!currentDirectory.Exists) 
        {
            return Result.Failure<string>(new Error($"Associated folder isn't exists."));
        }

        string newDirectoryName = GetDirectoryName(movie);
        string newDirecotryFullPath = Path.Combine(Path.GetDirectoryName(currentDirectory.FullName), newDirectoryName);
        currentDirectory.MoveTo(newDirecotryFullPath);

        await movie
           .ToJson()
           .AddSerializedJsonEntityToAsync(Path.Combine(currentDirectory.FullName, MovieEntityJson.FileName));

        return Result.Success(newDirecotryFullPath.GetRelational(_root));
    }

    private string GetDirectoryName(Movie movie)
    {
        return $"{movie.ProductionInfo?.Year} {DomainServicesConstants.MovieDirectoryNameSeparator} {movie.Title}";
    }
}