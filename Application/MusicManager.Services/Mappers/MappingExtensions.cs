using MusicManager.Domain.Models;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services.Mappers;

public static class MappingExtensions
{
    public static SongwriterDTO ToDTO(this Songwriter songwriter)
    {
        return new SongwriterDTO(
            songwriter.Id,
            songwriter.Name,
            songwriter.Surname,
            songwriter.Movies.Select(e => e.ToDTO()),
            songwriter.Compilations.Select(e => e.ToDTO())
            );
    }

    public static SongwriterLookupDTO ToLookupDTO(this Songwriter songwriter)
    {
        return new SongwriterLookupDTO(
            songwriter.Id,
            $"{songwriter.Name} {songwriter.Surname}"
            );
    }

    public static MovieDTO ToDTO(this Movie movie)
    {
        return new MovieDTO(
            movie.Id,
            movie.SongwriterId,
            movie.Title,
            movie.ProductionInfo.Country,
            movie.ProductionInfo.Year,
            movie.DirectorInfo?.Name,
            movie.DirectorInfo?.Surname,
            movie.Releases.Select(e => e.ToDTO())
            );
    }

    public static CompilationDTO ToDTO(this Compilation compilation)
    {
        return new CompilationDTO(
            compilation.Id,
            compilation.SongwriterId,
            compilation.Identifier,
            compilation.ProductionInfo.Country,
            compilation.ProductionInfo.Year,
            compilation.Type,
            compilation.Songs.Select(e => e.ToDTO())
            );
    }

    public static MovieReleaseDTO ToDTO(this MovieRelease movieRelease)
    {
        return new MovieReleaseDTO(
            movieRelease.Id,
            movieRelease.Identifier,
            movieRelease.ProductionInfo.Country,
            movieRelease.ProductionInfo.Year,
            movieRelease.Type,
            movieRelease.Songs.Select(e => e.ToDTO())
            );
    }

    public static SongDTO ToDTO(this Song song)
    {
        return new SongDTO(
            song.Id,
            song.DiscId,
            song.Name,
            song.Number,
            song.PlaybackInfo?.ExecutableType,
            song.DiscNumber,
            song.PlaybackInfo?.SongDuration
            );
    }
}
