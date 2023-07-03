using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Songwriter
{
    #region --Fields--

    private readonly List<Movie> _movies = new();

    private readonly List<Disc> _discs = new();

    #endregion

    #region --Properties--

    public SongwriterId Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Surname { get; private set; } = string.Empty;

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public IReadOnlyCollection<Movie> Movies => _movies.ToList();

    public IReadOnlyCollection<Disc> Discs => _discs.ToList();

    #endregion

    #region --Constructors--

    private Songwriter()
    {
        Id = SongwriterId.Create();
    }

    #endregion

    #region --Methods--

    public static Result<Songwriter> Create(
        string name, 
        string surname)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname)) 
        {
            return Result.Failure<Songwriter>(DomainErrors.NullOrEmptyStringPassedError());
        }

        return new Songwriter
        {
            Name = name,
            Surname = surname,
        };
    }

    public static Result<Songwriter> Create(
        string name, 
        string surname, 
        string directoryFullPath)
    {
        var creationResult = Create(name, surname);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var songwriter = creationResult.Value;
        var settingDirInfoResult = songwriter.SetDirectoryInfo(directoryFullPath);

        return settingDirInfoResult.IsFailure ?
            Result.Failure<Songwriter>(settingDirInfoResult.Error)
            :
            songwriter;
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

    public void AddMovie(Movie movie)
    {
        _movies.Add(movie);
    }

    public void AddDisc(Disc disc)
    {
        _discs.Add(disc);
    }

    #endregion
}

public record SongwriterId(Guid Value)
{
    public static SongwriterId Create() => new(Guid.NewGuid());
}
