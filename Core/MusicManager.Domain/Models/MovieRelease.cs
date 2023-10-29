using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class MovieRelease : Disc
{
	#region --Fields--

	private readonly List<MovieReleaseLink> _moviesLinks = new();

	#endregion

	#region --Properties--

	public IReadOnlyCollection<MovieReleaseLink> MoviesLinks => _moviesLinks.ToList();

	#endregion

	#region --Constructors--

	private MovieRelease() : base()
    {

    }

    #endregion

    #region --Methods--

    public static Result<MovieRelease> Create(
        DiscType discType, 
        string identifier,
        int? productionYear = null,
        string? productionCountry = null)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure<MovieRelease>(DomainErrors.NullOrEmptyStringPassed(nameof(identifier)));
        }

        var movieRelease = new MovieRelease()
        {
            Identifier = identifier,
        };

        var settingDiscTypeRes = movieRelease.SetDiscType(discType);
        if (settingDiscTypeRes.IsFailure)
        {
            return Result.Failure<MovieRelease>(settingDiscTypeRes.Error);
        }

        var settingProductionInfoResult = movieRelease.SetProductionInfo(productionCountry, productionYear);
        if (settingProductionInfoResult.IsFailure)
        {
            return Result.Failure<MovieRelease>(settingProductionInfoResult.Error);
        }

        return movieRelease;
    }

    public static Result<MovieRelease> Create(
        DiscType discType,
        string identifier,
        string associatedFolderPath,   
        int? productionYear = null,
        string? productionCountry = null)
    {
        var creationResult = Create(discType, identifier, productionYear, productionCountry);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var movieRelease = creationResult.Value;
		var settingDirectoryInfoResult = movieRelease.SetAssociatedFolder(associatedFolderPath);

        return settingDirectoryInfoResult.IsFailure ? 
            Result.Failure<MovieRelease>(settingDirectoryInfoResult.Error) : movieRelease;
    }

    internal void AddMovieLink(MovieReleaseLink movieLink)
    {
        _moviesLinks.Add(movieLink);
    }

    #endregion
}

