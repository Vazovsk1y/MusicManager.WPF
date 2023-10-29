using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Compilation : Disc
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongwriterId SongwriterId { get; }

    #endregion

    #region --Constructors--

    private Compilation(SongwriterId songwriterId) : base()
    {
        SongwriterId = songwriterId;
    }

    #endregion

    #region --Methods--

    public static Result<Compilation> Create(
        SongwriterId songwriterId,
        DiscType discType,
        string identifier,
        int? productionYear = null,
        string? productionCountry = null)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure<Compilation>(DomainErrors.NullOrEmptyStringPassed(nameof(identifier)));
        }

        var compilation = new Compilation(songwriterId)
        {
            Identifier = identifier,
        };

        var settingDiscTypeRes = compilation.SetDiscType(discType);
        if (settingDiscTypeRes.IsFailure)
        {
            return Result.Failure<Compilation>(settingDiscTypeRes.Error);
        }

        var settingProdInfoResult = compilation.SetProductionInfo(productionCountry, productionYear);
        if (settingProdInfoResult.IsFailure)
        {
            return Result.Failure<Compilation>(settingProdInfoResult.Error);
        }

        return compilation;
    }

    public static Result<Compilation> Create(
        SongwriterId songwriterId,
        DiscType discType,
        string identifier,
        string associatedFolderPath,
        int? productionYear = null,
        string? productionCountry = null)
    {
        var creationResult = Create(songwriterId, discType, identifier, productionYear, productionCountry);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var compilation = creationResult.Value;
		var settingDirectoryInfoResult = compilation.SetAssociatedFolder(associatedFolderPath);

        return settingDirectoryInfoResult.IsFailure ?
            Result.Failure<Compilation>(settingDirectoryInfoResult.Error) : compilation;
    }

    #endregion
}

