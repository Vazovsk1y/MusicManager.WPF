using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos
{
	public record MovieLinkDTO(MovieId MovieId, bool AddAsFolder = true);

    public record MovieReleaseAddDTO(
        IEnumerable<MovieLinkDTO> MoviesLinks,
        string Identifier,
        DiscType DiscType,
        int? ProductionYear,
        string? ProductionCountry
        );
}