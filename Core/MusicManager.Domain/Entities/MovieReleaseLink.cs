using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Entities;

public class MovieReleaseLink
{
    public MovieId MovieId { get; private set; }

    public DiscId MovieReleaseId { get; private set; }

    public MovieRelease MovieRelease { get; private set; }

    public Movie Movie { get; private set; }

    public EntityDirectoryInfo? ReleaseLinkInfo { get; private set; }

#pragma warning disable CS8618
	private MovieReleaseLink() { }

#pragma warning restore CS8618

	private MovieReleaseLink(MovieRelease movieRelease, Movie movie)
    {
        MovieRelease = movieRelease;
        Movie = movie;
        MovieId = movie.Id;
        MovieReleaseId = movieRelease.Id;
    }

    internal static Result<MovieReleaseLink> Create(MovieRelease movieRelease, Movie movie, string? moviReleaseLinkPath = null)
    {
        if (movie is null || movieRelease is null)
        {
            return Result.Failure<MovieReleaseLink>(DomainErrors.NullPassed("movie release or movie"));
        }

        var link = new MovieReleaseLink(movieRelease, movie);
        if (moviReleaseLinkPath is not null)
        {
            var entityDirectoryInfoResult = EntityDirectoryInfo.Create(moviReleaseLinkPath);
            if (entityDirectoryInfoResult.IsFailure)
            {
                return Result.Failure<MovieReleaseLink>(entityDirectoryInfoResult.Error);
            }
            link.ReleaseLinkInfo = entityDirectoryInfoResult.Value;
        }

        return link;
    }
}
