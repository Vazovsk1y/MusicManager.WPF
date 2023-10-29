using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts.Dtos;

public record MovieReleaseDTO : DiscDTO
{
    public MovieReleaseDTO(
        DiscId Id,
        string Identifier, 
        string? ProductionCountry, 
        int? ProductionYear, 
        DiscType DiscType) : base(Id, Identifier, ProductionCountry, ProductionYear, DiscType)
    {
    }
}
