using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Movie : IAggregateRoot
{
    #region --Fields--

    private readonly List<MovieReleaseLink> _releasesLinks = new();

    #endregion

    #region --Properties--

    public MovieId Id { get; init; }

    public SongwriterId SongwriterId { get; init; }

    public ProductionInfo ProductionInfo { get; private set; }

    public Director? Director { get; private set; } 

    public EntityDirectoryInfo? EntityDirectoryInfo { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public IReadOnlyCollection<MovieReleaseLink> ReleasesLinks => _releasesLinks.ToList();

    #endregion

    #region --Constructors--

    protected Movie() { } // for EF

    private Movie(SongwriterId songwriterId) 
    {
        SongwriterId = songwriterId;
        Id = MovieId.Create();
    }

    #endregion

    #region --Methods--

    public static Result<Movie> Create(
        SongwriterId songwriterId,
        string title, 
        int productionYear, 
        string? productionCountry = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Movie>(DomainErrors.NullOrEmptyStringPassed(nameof(title)));
        }

        var prodInfoResult = ProductionInfo.Create(productionCountry, productionYear);

        return prodInfoResult.IsFailure ? 
            Result.Failure<Movie>(prodInfoResult.Error)
            :
            new Movie(songwriterId)
            {
                Title = title,
                ProductionInfo = prodInfoResult.Value,
            };
    }

    public static Result<Movie> Create(
        SongwriterId songwriterId,
        string title,
        string directoryFullPath,
        int productionYear,
        string? productionCountry = null)
    {
        var creationResult = Create(songwriterId, title, productionYear, productionCountry);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var entityDirInfoRes = creationResult.Value.SetDirectoryInfo(directoryFullPath);

        return entityDirInfoRes.IsFailure ?
            Result.Failure<Movie>(entityDirInfoRes.Error)
            :
            creationResult.Value;
    }

    public Result SetDirectoryInfo(string path)
    {
        var result = EntityDirectoryInfo.Create(path);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        EntityDirectoryInfo = result.Value;
        return Result.Success();
    }

    public Result SetProductionInfo(string? productionCountry, int productionYear)
    {
        var result = ProductionInfo.Create(productionCountry, productionYear);

        if (result.IsSuccess)
        {
            ProductionInfo = result.Value;
            return Result.Success();
        }

        return Result.Failure(result.Error);
    }

    public Result AddRelease(MovieRelease release, bool checkDirectoryInfo = false, string? realeseLinkPath = null)
    {
        if (release is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed(nameof(release)));
        }

        if (_releasesLinks.SingleOrDefault(i => i.MovieRelease.Id == release.Id) is not null)
        {
            return Result.Failure(DomainErrors.EntityAlreadyExists(nameof(release)));
        }

        if (checkDirectoryInfo &&
            _releasesLinks.SingleOrDefault(m =>
            m.MovieRelease.EntityDirectoryInfo == release.EntityDirectoryInfo) is not null)
        {
            return Result.Failure(new Error($"MovieRelease with passed directory info is already exists."));
        }

        var linkCreationResult = MovieReleaseLink.Create(release, this, realeseLinkPath);
        if (linkCreationResult.IsFailure)
        {
            return Result.Failure(linkCreationResult.Error);
        }

        var addingDiscResult = release.AddMovieLink(linkCreationResult.Value);
        if (addingDiscResult.IsFailure)
        {
            return Result.Failure(addingDiscResult.Error);
        }

        _releasesLinks.Add(linkCreationResult.Value);
        return Result.Success();
    }

    public Result SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(DomainErrors.NullOrEmptyStringPassed(nameof(title)));
        }

        Title = title;  
        return Result.Success();
    }

    public Result SetDirector(Director director)
    {
        if (director is null)
        {
            return Result.Failure(DomainErrors.NullEntityPassed("director"));
        }

        Director?.RemoveMovie(Id);
        Director = director;
        director.AddMovie(this);
        return Result.Success();
    }

    #endregion
}

public record MovieId(Guid Value)
{
    public static MovieId Create() => new(Guid.NewGuid());
}
