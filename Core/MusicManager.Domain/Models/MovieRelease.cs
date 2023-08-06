using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class MovieRelease : Disc
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
        int productionYear,
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

