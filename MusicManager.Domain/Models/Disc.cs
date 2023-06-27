using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Disc
{
    #region --Fields--

    private readonly List<Song> _songs = new();

    #endregion

    #region --Properties--

    public DiscId Id { get; private set; }

    public SongwriterId SongwriterId { get; private set; }

    public MovieId? MovieId { get; private set; }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public DiscType Type { get; private set; }

    public string Identifier { get; private set; } = string.Empty;

    public IReadOnlyCollection<Song> Songs => _songs.ToList();  

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
        Movie movieParent,
        DiscType discType,
        string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure<Disc>(DomainErrors.NullOrEmptyStringPassedError(nameof(identifier)));
        }

        return new Disc(movieParent.SongwriterId)
        {
            Type = discType,
            MovieId = movieParent.Id,
            Identifier = identifier
        };
    }

    public static Result<Disc> Create(
        Movie parent,
        DiscType diskType, 
        string identifier, 
        string directoryFullPath, 
        string productionYear, 
        string productionCountry)
    {
        var diskCreationResult = Create(parent, diskType, identifier);

        if (diskCreationResult.IsFailure)
        {
            return diskCreationResult;
        }

        var disk = diskCreationResult.Value;
        var settingProdInfoResult = disk.SetProductionInfo(productionCountry, productionYear);

        if (settingProdInfoResult.IsFailure)
        {
            return Result.Failure<Disc>(settingProdInfoResult.Error);
        }

        var settingDirectoryInfoResult = disk.SetDirectoryInfo(directoryFullPath);

        return settingDirectoryInfoResult.IsSuccess ?
            disk : Result.Failure<Disc>(settingDirectoryInfoResult.Error);
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

    #endregion
}

public record DiscId(Guid Value)
{
    public static DiscId Create() => new(Guid.NewGuid());
}
