using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Disc : IAggregateRoot
{
    #region --Fields--

    private readonly List<Song> _songs = new();

    private readonly List<Movie> _movies = new();

    private readonly List<Cover> _covers = new();

    #endregion

    #region --Properties--

    public DiscId Id { get; private set; }

    public SongwriterId SongwriterId { get; private set; }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public DiscType Type { get; private set; }

    public string Identifier { get; private set; } = string.Empty;

    public IReadOnlyCollection<Song> Songs => _songs.ToList();
    
    public IReadOnlyCollection<Movie> Movies => _movies.ToList();

    public IReadOnlyCollection<Cover> Covers => _covers.ToList();

    #endregion

    #region --Constructors--

    private Disc(SongwriterId songwriterId) 
    {
        SongwriterId = songwriterId; 
        Id = DiscId.Create();
        ProductionInfo = ProductionInfo.Undefined;
    }

    #endregion

    #region --Methods--

    public static Result<Disc> Create(
        SongwriterId songwriterId,
        DiscType discType, 
        string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure<Disc>(DomainErrors.NullOrEmptyStringPassedError(nameof(identifier)));
        }

        return new Disc(songwriterId) 
        {
            Type = discType, 
            Identifier = identifier 
        };
    }

    public static Result<Disc> Create(
        SongwriterId songwriterId,
        DiscType discType,
        string identifier,
        string directoryFullPath)
    {
        var creationResult = Create(songwriterId, discType, identifier);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var settingDirectoryInfoResult = creationResult.Value.SetDirectoryInfo(directoryFullPath);

        return settingDirectoryInfoResult.IsFailure ? 
            Result.Failure<Disc>(settingDirectoryInfoResult.Error) : creationResult.Value;
    }

    public static Result<Disc> Create(
        SongwriterId songwriterId,
        DiscType discType,
        string identifier,
        string directoryFullPath,
        string productionYear,
        string productionCountry)
    {
        var diskCreationResult = Create(songwriterId, discType, identifier);

        if (diskCreationResult.IsFailure)
        {
            return diskCreationResult;
        }

        var settingDirectoryInfoResutlt = diskCreationResult.Value.SetDirectoryInfo(directoryFullPath);

        if (settingDirectoryInfoResutlt.IsFailure)
        {
            Result.Failure<Disc>(settingDirectoryInfoResutlt.Error);
        }

        var settingProdInfoResult = diskCreationResult.Value.SetProductionInfo(productionCountry, productionYear);

        return settingProdInfoResult.IsSuccess ?
            diskCreationResult.Value : Result.Failure<Disc>(settingProdInfoResult.Error);
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

    public void AddSong(Song song)
    {
        _songs.Add(song);
    }

    public void AddMovie(Movie movie)
    {
        _movies.Add(movie);
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

    #endregion
}

public record DiscId(Guid Value)
{
    public static DiscId Create() => new(Guid.NewGuid());
}
