using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Movie 
{
    #region --Fields--

    private readonly List<Disc> _discs = new();

    #endregion

    #region --Properties--

    public MovieId Id { get; private set; }

    public SongwriterId SongwriterId { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public DirectorInfo DirectorInfo { get; private set; }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public IReadOnlyCollection<Disc> Discs => _discs.ToList();

    #endregion

    #region --Constructors--

    private Movie(SongwriterId songwriterId) 
    { 
        ProductionInfo = ProductionInfo.Undefined;
        DirectorInfo = DirectorInfo.Undefined;
        Id = MovieId.Create();
        SongwriterId = songwriterId;
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
            return Result.Failure<Movie>(DomainErrors.NullOrEmptyStringPassedError(nameof(title)));
        }

        var prodInfoResult = ProductionInfo.Create(productionCountry, productionYear);

        return prodInfoResult.IsFailure ? Result.Failure<Movie>(prodInfoResult.Error)
            :
            new Movie(songwriterId)
            {
                Title = title,
                ProductionInfo = prodInfoResult.Value
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

    #endregion
}

public record MovieId(Guid Value)
{
    public static MovieId Create() => new(Guid.NewGuid());
}
