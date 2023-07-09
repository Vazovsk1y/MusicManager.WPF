using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Compilation : Disc
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

    #endregion
}

