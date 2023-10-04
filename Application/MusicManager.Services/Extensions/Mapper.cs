using MusicManager.Domain.Entities;
using MusicManager.Domain.Models;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services.Extensions;

public static class Mapper
{
    public static SongwriterDTO ToDTO(this Songwriter songwriter)
    {
        return new SongwriterDTO(
            songwriter.Id,
            songwriter.Name,
            songwriter.LastName
            );
    }

    public static SongwriterLookupDTO ToLookupDTO(this Songwriter songwriter)
    {
        return new SongwriterLookupDTO(
            songwriter.Id,
            $"{songwriter.Name} {songwriter.LastName}"
            );
    }

    public static MovieDTO ToDTO(this Movie movie)
    {
        return new MovieDTO(
            movie.Id,
            movie.SongwriterId,
            movie.Title,
            movie.ProductionInfo?.Country,
            (int)movie.ProductionInfo!.Year!,
            movie.Director is null ? null : new DirectorDTO(movie.Director.Id, movie.Director.FullName)
            );
    }

    public static MovieReleaseLinkDTO ToDTO(this MovieReleaseLink movieReleaseLink)
    {
        return new MovieReleaseLinkDTO(movieReleaseLink.MovieRelease.ToDTO(), movieReleaseLink.ReleaseLinkInfo is null);
    }

    public static MovieLookupDTO ToLookupDTO(this Movie movie)
    {
        return new MovieLookupDTO(
            movie.Id,
            movie.Title,
            (int)movie.ProductionInfo.Year!
            );
    }

    public static CompilationDTO ToDTO(this Compilation compilation)
    {
        return new CompilationDTO(
            compilation.Id,
            compilation.SongwriterId,
            compilation.Identifier,
            compilation.ProductionInfo?.Country,
            compilation.ProductionInfo?.Year,
            compilation.Type
            );
    }

    public static MovieReleaseDTO ToDTO(this MovieRelease movieRelease)
    {
        return new MovieReleaseDTO(
            movieRelease.Id,
            movieRelease.Identifier,
            movieRelease.ProductionInfo?.Country,
            movieRelease.ProductionInfo?.Year,
            movieRelease.Type
            );
    }

    public static SongDTO ToDTO(this Song song)
    {
        return new SongDTO(
            song.Id,
            song.DiscId,
            song.Title,
            song.Order,
            song.PlaybackInfo.AudioType,
            song.DiscNumber?.Value,
            song.PlaybackInfo.Duration
            );
    }
}
