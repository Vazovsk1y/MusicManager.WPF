using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Songwriter : IAggregateRoot
{
    #region --Fields--

    private readonly List<Movie> _movies = new();

    private readonly List<Compilation> _compilations = new();

    #endregion

    #region --Properties--

    public SongwriterId Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Surname { get; private set; } = string.Empty;

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public IReadOnlyCollection<Compilation> Compilations => _compilations.ToList();

    public IReadOnlyCollection<Movie> Movies => _movies.ToList(); 

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
            return Result.Failure<Songwriter>(DomainErrors.NullOrEmptyStringPassed());
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

    public Result AddMovie(Movie movie, bool checkDirectoryInfo = false)
    {
        if (movie is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed(nameof(movie)));
        }

        if (_movies.SingleOrDefault(i => i.Id == movie.Id) is not null)
        {
            return Result.Failure(DomainErrors.EntityAlreadyExists(nameof(movie)));
        }

        if (checkDirectoryInfo && _movies.SingleOrDefault(m =>
            m.EntityDirectoryInfo == movie.EntityDirectoryInfo) is not null)
        {
            return Result.Failure(new Error("Movie with passed directory info is already exists."));
        }

        _movies.Add(movie);
        return Result.Success();
    }

    public Result AddCompilation(Compilation disc, bool checkDirectoryInfo = false)
    {
        if (disc is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed(nameof(disc)));
        }

        if (_compilations.SingleOrDefault(i => i.Id == disc.Id) is not null)
        {
            return Result.Failure(DomainErrors.EntityAlreadyExists(nameof(disc)));
        }

        if (checkDirectoryInfo && _compilations.SingleOrDefault(m =>
        m.EntityDirectoryInfo == disc.EntityDirectoryInfo) is not null)
        {
            return Result.Failure(new Error($"Compilation with passed directory info is already exists."));
        }

        _compilations.Add(disc);
        return Result.Success();
    }

    #endregion
}

public record SongwriterId(Guid Value)
{
    public static SongwriterId Create() => new(Guid.NewGuid());
}
