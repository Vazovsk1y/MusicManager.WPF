using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Songwriter : Entity
{
    #region --Fields--

    private readonly List<Movie> _movies = new();

    #endregion

    #region --Properties--

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Surname { get; private set; } = string.Empty;

    public IReadOnlyCollection<Movie> Movies => _movies.ToList();

    #endregion

    #region --Constructors--

    private Songwriter() : base() { }

    #endregion

    #region --Methods--

    public static Result<Songwriter> Create(string name, string surname)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname)) 
        {
            return Result.Failure<Songwriter>(DomainErrors.NullOrEmptyStringPassedError());
        }

        return new Songwriter
        {
            Name = name,
            Surname = surname,
        };
    }

    public static Result<Songwriter> Create(string name, string surname, string directoryName, string directoryFullPath)
    {
        var creationResult = Create(name, surname);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var settingDirInfoResult = creationResult.Value.SetDirectoryInfo(directoryName, directoryFullPath);

        return settingDirInfoResult.IsFailure ? 
            Result.Failure<Songwriter>(settingDirInfoResult.Error) 
            : 
            creationResult.Value;
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
