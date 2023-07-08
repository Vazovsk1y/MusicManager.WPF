using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Movie : IAggregateRoot
{
    #region --Fields--

    private readonly List<MovieRelease> _movieReleases = new();

    #endregion

    #region --Properties--

    public MovieId Id { get; private set; }

    public SongwriterId SongwriterId { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public DirectorInfo DirectorInfo { get; private set; } 

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public IReadOnlyCollection<MovieRelease> Releases => _movieReleases.ToList();

    #endregion

    #region --Constructors--

    private Movie(SongwriterId songwriterId) 
    {
        SongwriterId = songwriterId;
        Id = MovieId.Create();
    }

    #endregion

    #region --Methods--

    public static Result<Movie> Create(
        SongwriterId songwriterId,
        string title, 
        string productionYear, 
        string productionCountry)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Movie>(DomainErrors.NullOrEmptyStringPassed(nameof(title)));
        }

        var prodInfoResult = ProductionInfo.Create(productionCountry, productionYear);

        return prodInfoResult.IsFailure ? Result.Failure<Movie>(prodInfoResult.Error)
            :
            new Movie(songwriterId)
            {
                Title = title,
                ProductionInfo = prodInfoResult.Value,
                DirectorInfo = DirectorInfo.Undefined,
            };
    }

    public static Result<Movie> Create(
        SongwriterId songwriterId,
        string title, 
        string productionYear, 
        string productionCountry, 
        string directoryFullPath)
    {
        var creationResult = Create(songwriterId, title, productionYear, productionCountry);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var settingDirInfoResult = creationResult.Value.SetDirectoryInfo(directoryFullPath);

        return settingDirInfoResult.IsFailure ?
            Result.Failure<Movie>(settingDirInfoResult.Error)
            :
            creationResult.Value;
    }

    public Result SetDirectoryInfo(string fullPath)
    {
        var result = EntityDirectoryInfo.Create(fullPath);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        EntityDirectoryInfo = result.Value;
        return Result.Success();
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

    public Result AddRelease(MovieRelease release)
    {
        if (release is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed(nameof(release)));
        }

        if (_movieReleases.SingleOrDefault(i => i.Id == release.Id) is not null)
        {
            return Result.Failure(DomainErrors.EntityAlreadyExists(nameof(release)));
        }

        var addingDiscResult = release.AddMovie(this);
        if (addingDiscResult.IsFailure)
        {
            return Result.Failure(addingDiscResult.Error);
        }

        _movieReleases.Add(release);
        return Result.Success();
    }

    #endregion
}

public record MovieId(Guid Value)
{
    public static MovieId Create() => new(Guid.NewGuid());
}
