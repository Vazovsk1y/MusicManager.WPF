using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos
{
    public record MovieDTO(
        MovieId Id,
        SongwriterId SongwriterId,
        string Title,
        string ProductionCountry,
        string ProductionYear,
        string? DirectorName,
        string? DirectorLastName,
        IEnumerable<MovieReleaseDTO> MovieReleasesDTOs);
}