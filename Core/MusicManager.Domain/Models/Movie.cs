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

    public string Title { get; private set; }

    public Director? Director { get; private set; } 

    public EntityDirectoryInfo? AssociatedFolderInfo { get; private set; }

    public IReadOnlyCollection<MovieReleaseLink> ReleasesLinks => _releasesLinks.ToList();

	#endregion

	#region --Constructors--

#pragma warning disable CS8618 
	protected Movie() { } // for EF

	private Movie(SongwriterId songwriterId) 
    {
        SongwriterId = songwriterId;
        Id = MovieId.Create();
    }
#pragma warning restore CS8618 

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
        string associatedFolderPath,
        int productionYear,
        string? productionCountry = null)
    {
        var creationResult = Create(songwriterId, title, productionYear, productionCountry);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var entityDirInfoRes = creationResult.Value.SetAssociatedFolder(associatedFolderPath);

        return entityDirInfoRes.IsFailure ?
            Result.Failure<Movie>(entityDirInfoRes.Error)
            :
            creationResult.Value;
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

    public Result SetProductionInfo(string? productionCountry, int productionYear)
    {
		var result = ProductionInfo.Create(productionCountry, productionYear);

		if (result.IsFailure)
		{
			return Result.Failure(result.Error);
		}

		ProductionInfo = result.Value;
		return Result.Success();
	}

	public Result AddRelease(MovieRelease release, string? releaseLinkPath = null)
    {
        if (release is null)
        {
            return Result.Failure(DomainErrors.NullPassed(nameof(release)));
        }

        if (IsReleaseExists())
        {
            return Result.Failure(DomainErrors.PassedEntityAlreadyAdded(nameof(release)));
        }

        var linkCreationResult = MovieReleaseLink.Create(release, this, releaseLinkPath);
        if (linkCreationResult.IsFailure)
        {
            return Result.Failure(linkCreationResult.Error);
        }

        var link = linkCreationResult.Value;
		release.AddMovieLink(link);
        _releasesLinks.Add(link);
        return Result.Success();

        bool IsReleaseExists()
        {
            return _releasesLinks.SingleOrDefault(lnk => 
            lnk.MovieReleaseId == release.Id
            || 
            (lnk.ReleaseLinkInfo?.Path == releaseLinkPath && lnk.MovieRelease.AssociatedFolderInfo == release.AssociatedFolderInfo)) is not null;
		}
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
            return Result.Failure(DomainErrors.NullPassed("director"));
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
