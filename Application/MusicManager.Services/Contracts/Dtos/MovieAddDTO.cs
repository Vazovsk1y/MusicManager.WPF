using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos
{
    public record MovieAddDTO(
        SongwriterId SongwriterId,
        int ProductionYear,
        string ProductionCountry,
        string Title);
}