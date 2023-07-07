using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class MovieRelease : Disc, IAggregateRoot 
{
    #region --Fields--

    private readonly List<Movie> _movies = new();

    #endregion

    #region --Properties--

    public IReadOnlyCollection<Movie> Movies => _movies.ToList();

    #endregion

    #region --Constructors--

    private MovieRelease() : base()
    {
        ProductionInfo = ProductionInfo.Undefined;
    }

    #endregion

    #region --Methods--

    public static Result<MovieRelease> Create(
        DiscType discType, 
        string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure<MovieRelease>(DomainErrors.NullOrEmptyStringPassed(nameof(identifier)));
        }

        return new MovieRelease() 
        {
            Type = discType, 
            Identifier = identifier 
        };
    }

    public static Result<MovieRelease> Create(
        DiscType discType,
        string identifier,
        string directoryFullPath)
    {
        var creationResult = Create(discType, identifier);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var settingDirectoryInfoResult = creationResult.Value.SetDirectoryInfo(directoryFullPath);

        return settingDirectoryInfoResult.IsFailure ? 
            Result.Failure<MovieRelease>(settingDirectoryInfoResult.Error) : creationResult.Value;
    }

    public static Result<MovieRelease> Create(
        DiscType discType,
        string identifier,
        string directoryFullPath,
        string productionYear,
        string productionCountry)
    {
        var diskCreationResult = Create(discType, identifier);

        if (diskCreationResult.IsFailure)
        {
            return diskCreationResult;
        }

        var settingDirectoryInfoResutlt = diskCreationResult.Value.SetDirectoryInfo(directoryFullPath);

        if (settingDirectoryInfoResutlt.IsFailure)
        {
            Result.Failure<MovieRelease>(settingDirectoryInfoResutlt.Error);
        }

        var settingProdInfoResult = diskCreationResult.Value.SetProductionInfo(productionCountry, productionYear);

        return settingProdInfoResult.IsSuccess ?
            diskCreationResult.Value : Result.Failure<MovieRelease>(settingProdInfoResult.Error);
    }

    public Result SetDirectoryInfo(string fullPath)
    {
        var result = EntityDirectoryInfo.Create(fullPath);

        if (result.IsSuccess)
        {
            EntityDirectoryInfo = result.Value;
            return result;
        }

        return result;
    }

    public Result SetProductionInfo(string productionCountry, string productionYear)
    {
        var result = ProductionInfo.Create(productionCountry, productionYear);

        if (result.IsSuccess)
        {
            ProductionInfo = result.Value;
            return Result.Success();
        }

        return Result.Failure(result.Error);
    }

    public Result AddSong(Song song)
    {
        if (song is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed(nameof(song)));
        }

        if (_songs.SingleOrDefault(i => i.Id == song.Id) is not null)
        {
            return Result.Failure(DomainErrors.EntityAlreadyExists(nameof(song)));
        }

        _songs.Add(song);
        return Result.Success();
    }

    public Result AddCover(string coverPath)
    {
        var coverCreationResult = Cover.Create(Id, coverPath);
        if (coverCreationResult.IsSuccess)
        {
            _covers.Add(coverCreationResult.Value);
            return Result.Success();
        }
        return Result.Failure(coverCreationResult.Error);
    }

    internal Result AddMovie(Movie movie)
    {
        if (_movies.SingleOrDefault(i => i.Id == movie.Id) is not null)
        {
            return Result.Failure(DomainErrors.EntityAlreadyExists(nameof(movie)));
        }

        _movies.Add(movie);
        return Result.Success();
    }

    #endregion
}

