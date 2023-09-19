using MusicManager.Domain.Enums;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos
{
    public record MovieReleaseAddDTO(
        IEnumerable<MovieId> MoviesLinks,
        string Identifier,
        DiscType DiscType,
        int? ProductionYear,
        string? ProductionCountry
        );
}