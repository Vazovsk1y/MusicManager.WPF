using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Movie : Entity
{
    #region --Fields--

    private readonly List<Disc> _discs = new();

    #endregion

    #region --Properties--

    public Guid SongwriterId { get; private set; }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public DirectorInfo? DirectorInfo { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public IReadOnlyCollection<Disc> Discs => _discs.ToList();

    #endregion

    #region --Constructors--

    private Movie() : base() { ProductionInfo = ProductionInfo.None; }

    #endregion

    #region --Methods--

    public static Result<Movie> Create(
        string title, 
        string productionYear, 
        string productionCountry)
    {
        var prodInfoResult = ProductionInfo.Create(productionCountry, productionYear);

        return prodInfoResult.IsFailure ? Result.Failure<Movie>(prodInfoResult.Error) : new Movie
        {
            Title = title,
            ProductionInfo = prodInfoResult.Value
        };
    }

    public static Result<Movie> Create(
        string title, 
        string productionYear, 
        string productionCountry, 
        string directoryName, 
        string directoryFullPath)
    {
        var creationResult = Create(title, productionYear, productionCountry);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var settingDirInfoResult = creationResult.Value.SetDirectoryInfo(directoryName, directoryFullPath);

        return settingDirInfoResult.IsFailure ?
            Result.Failure<Movie>(settingDirInfoResult.Error)
            :
            creationResult.Value;
    }

    public Result SetParent(Songwriter songwriter)
    {
        SongwriterId = songwriter.Id;
        return Result.Success();
    }

    public Result SetDirectoryInfo(string name, string fullPath)
    {
        var result = EntityDirectoryInfo.Create(name, fullPath);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        EntityDirectoryInfo = result.Value;
        return Result.Success();
    }

    #endregion
}
