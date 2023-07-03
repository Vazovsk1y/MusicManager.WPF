using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Movie : IAggregateRoot
{
    #region --Fields--

    private readonly List<Songwriter> _songwriters = new();

    private readonly List<Disc> _discs = new();

    #endregion

    #region --Properties--

    public MovieId Id { get; private set; }

    public ProductionInfo ProductionInfo { get; private set; }

    public DirectorInfo DirectorInfo { get; private set; }

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public IReadOnlyCollection<Disc> Discs => _discs.ToList();

    public IReadOnlyCollection<Songwriter> Songwriters => _songwriters.ToList();

    #endregion

    #region --Constructors--

    private Movie() 
    { 
        ProductionInfo = ProductionInfo.Undefined;
        DirectorInfo = DirectorInfo.Undefined;
        Id = MovieId.Create();
    }

    #endregion

    #region --Methods--

    public static Result<Movie> Create(
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
            new Movie()
            {
                Title = title,
                ProductionInfo = prodInfoResult.Value
            };
    }

    public static Result<Movie> Create(
        string title, 
        string productionYear, 
        string productionCountry, 
        string directoryFullPath)
    {
        var creationResult = Create(title, productionYear, productionCountry);
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

    public void AddDisc(Disc disc)
    {
        _discs.Add(disc);
    }

    public void AddSongwriter(Songwriter songwriter)
    {
        _songwriters.Add(songwriter);
    }

    #endregion
}

public record MovieId(Guid Value)
{
    public static MovieId Create() => new(Guid.NewGuid());
}
