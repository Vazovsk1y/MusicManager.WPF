using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos
{
    public record MovieAddDTO(
        SongwriterId SongwriterId,
        string ProductionYear,
        string ProductionCountry,
        string Title);
}