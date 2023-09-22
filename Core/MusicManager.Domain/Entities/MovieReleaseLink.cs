using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Entities;

public class MovieReleaseLink
{
    public MovieId MovieId { get; }

    public DiscId DiscId { get; }
    public MovieRelease MovieRelease { get; }

    public Movie Movie { get; }

    public EntityDirectoryInfo? ReleaseLink { get; private set; }

    private MovieReleaseLink() { }
    private MovieReleaseLink(MovieRelease movieRelease, Movie movie)
    {
        MovieRelease = movieRelease;
        Movie = movie;
        MovieId = movie.Id;
        DiscId = movieRelease.Id;
    }

    internal static Result<MovieReleaseLink> Create(MovieRelease movieRelease, Movie movie, string? linkPath = null)
    {
        if (movie is null || movieRelease is null)
        {
            return Result.Failure<MovieReleaseLink>(DomainErrors.NullEntityPassed($"{nameof(movieRelease)} or {nameof(movie)}"));
        }

        var result = new MovieReleaseLink(movieRelease, movie);
        if (linkPath is not null)
        {
            var entityDirInfoRes = EntityDirectoryInfo.Create(linkPath);
            if (entityDirInfoRes.IsFailure)
            {
                return Result.Failure<MovieReleaseLink>(entityDirInfoRes.Error);
            }
            result.ReleaseLink = entityDirInfoRes.Value;
        }

        return result;
    }
}
