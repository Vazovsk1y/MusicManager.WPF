using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Compilation : Disc, IAggregateRoot
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongwriterId SongwriterId { get; private set; }

    #endregion

    #region --Constructors--

    private Compilation(SongwriterId songwriterId) : base()
    {
        SongwriterId = songwriterId;
        ProductionInfo = ProductionInfo.Undefined;
    }

    #endregion

    #region --Methods--

    public static Result<Compilation> Create(
        SongwriterId songwriterId,
        DiscType discType,
        string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure<Compilation>(DomainErrors.NullOrEmptyStringPassed(nameof(identifier)));
        }

        return new Compilation(songwriterId)
        {
            Type = discType,
            Identifier = identifier
        };
    }

    public static Result<Compilation> Create(
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
            Result.Failure<Compilation>(settingDirectoryInfoResult.Error) : creationResult.Value;
    }

    public static Result<Compilation> Create(
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
            Result.Failure<Compilation>(settingDirectoryInfoResutlt.Error);
        }

        var settingProdInfoResult = diskCreationResult.Value.SetProductionInfo(productionCountry, productionYear);

        return settingProdInfoResult.IsSuccess ?
            diskCreationResult.Value : Result.Failure<Compilation>(settingProdInfoResult.Error);
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

    #endregion
}


