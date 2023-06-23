using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Disc : Entity
{
    #region --Fields--

    private readonly List<Song> _songs = new();

    #endregion

    #region --Properties--

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public Guid? MovieId { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public DiscType Type { get; private set; }

    public string Identifier { get; private set; } = string.Empty;

    public IReadOnlyCollection<Song> Songs => _songs.ToList();  

    #endregion

    #region --Constructors--

    private Disc() : base()
    {
        ProductionInfo = ProductionInfo.None;
    }

    #endregion

    #region --Methods--

    public static Result<Disc> Create(
        DiscType discType, 
        string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            return Result.Failure<Disc>(DomainErrors.NullOrEmptyStringPassedError(nameof(identifier)));
        }

        return new Disc 
        {
            Type =discType, 
            Identifier = identifier 
        };
    }

    public static Result<Disc> Create(
        DiscType discType, 
        string identifier, 
        string directoryName, 
        string directoryFullPath)
    {
        var diskCreationResult = Create(discType, identifier);

        if (diskCreationResult.IsFailure)
        {
            return diskCreationResult;
        }

        var directorySettingInfoResult = diskCreationResult.Value.SetDirectoryInfo(directoryName, directoryFullPath);

        return directorySettingInfoResult.IsSuccess ? 
            diskCreationResult.Value : Result.Failure<Disc>(directorySettingInfoResult.Error);
    }

    public static Result<Disc> Create(
        string discTypeRow, 
        string identifier, 
        string directoryName, 
        string directoryFullPath)
    {
        var diskTypeMappingResult = discTypeRow.CreateDiscType();
        if (diskTypeMappingResult.IsFailure)
        {
            return Result.Failure<Disc>(diskTypeMappingResult.Error);
        }

        var diskCreationResult = Create(diskTypeMappingResult.Value, identifier);
        if (diskCreationResult.IsFailure)
        {
            return diskCreationResult;
        }

        var directorySettingInfoResult = diskCreationResult.Value.SetDirectoryInfo(directoryName, directoryFullPath);

        return directorySettingInfoResult.IsSuccess ?
            diskCreationResult.Value : Result.Failure<Disc>(directorySettingInfoResult.Error);
    }

    public static Result<Disc> Create(
        string discTypeRow, 
        string identifier, 
        string directoryName, 
        string directoryFullPath, 
        string productionYear, 
        string productionCountry)
    {
        var diskTypeMappingResult = discTypeRow.CreateDiscType();
        if (diskTypeMappingResult.IsFailure)
        {
            return Result.Failure<Disc>(diskTypeMappingResult.Error);
        }

        var diskCreationResult = Create(diskTypeMappingResult.Value, identifier, directoryName, directoryFullPath);

        if (diskCreationResult.IsFailure)
        {
            return diskCreationResult;
        }

        var settingProdInfoResult = diskCreationResult.Value.SetProductionInfo(productionCountry, productionYear);

        return settingProdInfoResult.IsSuccess ?
            diskCreationResult.Value : Result.Failure<Disc>(settingProdInfoResult.Error);
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

    public Result SetDirectoryInfo(string name, string fullPath)
    {
        var result = EntityDirectoryInfo.Create(name, fullPath);

        if (result.IsSuccess)
        {
            EntityDirectoryInfo = result.Value;
            return result;
        }

        return result;
    }

    #endregion
}
