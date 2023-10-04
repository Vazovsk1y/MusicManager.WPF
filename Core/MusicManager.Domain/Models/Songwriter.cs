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

    public SongwriterId Id { get; }

    public string Name { get; private set; } 

    public string LastName { get; private set; }

    public EntityDirectoryInfo? AssociatedFolderInfo { get; private set; }

    public IReadOnlyCollection<Compilation> Compilations => _compilations.ToList();

    public IReadOnlyCollection<Movie> Movies => _movies.ToList();

	#endregion

	#region --Constructors--

#pragma warning disable CS8618
	private Songwriter()
	{
        Id = SongwriterId.Create();
    }

#pragma warning restore CS8618

	#endregion

	#region --Methods--

	public static Result<Songwriter> Create(
        string name, 
        string lastName)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(lastName)) 
        {
            return Result.Failure<Songwriter>(DomainErrors.NullOrEmptyStringPassed());
        }

        return new Songwriter
        {
            Name = name,
            LastName = lastName,
        };
    }

    public static Result<Songwriter> Create(
        string name, 
        string lastName, 
        string associatedFolderPath)
    {
        var creationResult = Create(name, lastName);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }
        
        var songwriter = creationResult.Value;
        var settingDirInfoResult = songwriter.SetAssociatedFolder(associatedFolderPath);

        return settingDirInfoResult.IsFailure ?
            Result.Failure<Songwriter>(settingDirInfoResult.Error)
            :
            songwriter;
    }

    public Result SetAssociatedFolder(string path)
    {
        var result = EntityDirectoryInfo.Create(path);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        AssociatedFolderInfo = result.Value;
        return Result.Success();
    }

    public Result AddMovie(Movie movie)
    {
        if (movie is null)
        {
            return Result.Failure(DomainErrors.NullPassed(nameof(movie)));
        }

        if (_movies.SingleOrDefault(m => m.Id == movie.Id || m.AssociatedFolderInfo == movie.AssociatedFolderInfo) is not null)
        {
            return Result.Failure(DomainErrors.PassedEntityAlreadyAdded(nameof(movie)));
        }

        _movies.Add(movie);
        return Result.Success();
    }

    public Result AddCompilation(Compilation compilation)
    {
        if (compilation is null)
        {
            return Result.Failure(DomainErrors.NullPassed(nameof(compilation)));
        }

        if (_compilations.SingleOrDefault(c => c.Id == compilation.Id || c.AssociatedFolderInfo == compilation.AssociatedFolderInfo) is not null)
        {
            return Result.Failure(DomainErrors.PassedEntityAlreadyAdded(nameof(compilation)));
        }

        _compilations.Add(compilation);
        return Result.Success();
    }

    #endregion
}

public record SongwriterId(Guid Value)
{
    public static SongwriterId Create() => new(Guid.NewGuid());
}
