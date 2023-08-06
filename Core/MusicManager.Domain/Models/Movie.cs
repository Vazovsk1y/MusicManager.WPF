using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Movie : IAggregateRoot
{
    #region --Fields--

    private readonly List<MovieRelease> _releases = new();

    #endregion

    #region --Properties--

    public MovieId Id { get; private set; }

    public SongwriterId SongwriterId { get; private set; }

    public ProductionInfo? ProductionInfo { get; private set; }

    public DirectorInfo? DirectorInfo { get; private set; } 

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public IReadOnlyCollection<MovieRelease> Releases => _releases.ToList();

    #endregion

    #region --Constructors--

    protected Movie() { } // for EF

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
        int productionYear, 
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
            };
    }

    public static Result<Movie> Create(
        SongwriterId songwriterId,
        string title, 
        int productionYear, 
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

    public Result SetProductionInfo(string productionCountry, int productionYear)
    {
        var result = ProductionInfo.Create(productionCountry, productionYear);

        if (result.IsSuccess)
        {
            ProductionInfo = result.Value;
            return Result.Success();
        }

        return Result.Failure(result.Error);
    }

    public Result AddRelease(MovieRelease release, bool checkDirectoryInfo = false)
    {
        if (release is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed(nameof(release)));
        }

        if (_releases.SingleOrDefault(i => i.Id == release.Id) is not null)
        {
            return Result.Failure(DomainErrors.EntityAlreadyExists(nameof(release)));
        }

        if (checkDirectoryInfo &&
            _releases.SingleOrDefault(m =>
            m.EntityDirectoryInfo == release.EntityDirectoryInfo
            && m.Id == release.Id) is not null)
        {
            return Result.Failure(new Error($"MovieRelease with passed directory info is already exists."));
        }

        var addingDiscResult = release.AddMovie(this);
        if (addingDiscResult.IsFailure)
        {
            return Result.Failure(addingDiscResult.Error);
        }

        _releases.Add(release);
        return Result.Success();
    }

    #endregion
}

public record MovieId(Guid Value)
{
    public static MovieId Create() => new(Guid.NewGuid());
}
