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

        var settingResult = movieRelease.SetProductionInfo(productionCountry, productionYear);
        if (settingResult.IsFailure)
        {
            return Result.Failure<MovieRelease>(settingResult.Error);
        }

        return movieRelease;
    }

    public static Result<MovieRelease> Create(
        DiscType discType,
        string identifier,
        string directoryFullPath,   
        int? productionYear = null,
        string? productionCountry = null)
    {
        var creationResult = Create(discType, identifier, productionYear, productionCountry);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var settingDirectoryInfoResult = creationResult.Value.SetDirectoryInfo(directoryFullPath);

        return settingDirectoryInfoResult.IsFailure ? 
            Result.Failure<MovieRelease>(settingDirectoryInfoResult.Error) : creationResult.Value;
    }

    internal Result AddMovieLink(MovieReleaseLink movieLink)
    {

        _moviesLinks.Add(movieLink);
        return Result.Success();
    }

    #endregion
}

