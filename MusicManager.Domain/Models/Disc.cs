using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Errors;
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

    public DiscType DiscType { get; private set; }

    public string Identifier { get; private set; } = string.Empty;

    public IReadOnlyCollection<Song> Songs => _songs.ToList();  

    #endregion

    #region --Constructors--

    private Disc(
        DiscType discType,
        string identifier) : base()
    {
        DiscType = discType;
        Identifier = identifier;
        ProductionInfo = ProductionInfo.None;
    }

    #endregion

    #region --Methods--

    public static Result<Disc> Create(DiscType discType, string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            return Result.Failure<Disc>(DomainErrors.NullOrEmptyStringPassedError(nameof(identifier)));
        }

        return new Disc(discType, identifier);
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
