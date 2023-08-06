using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;

namespace MusicManager.Services.Contracts.Dtos
{
    public abstract record DiscDTO(
        DiscId Id,
        string Identifier,
        string ProductionCountry,
        int? ProductionYear,
        DiscType DiscType,
        IEnumerable<SongDTO> SongDTOs
        );
}