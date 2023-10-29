using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts.Dtos;

public record CompilationDTO : DiscDTO
{
    public SongwriterId SongwriterId { get; }

    public CompilationDTO(
        DiscId Id,
        SongwriterId songwriterId,
        string Identifier, 
        string? ProductionCountry, 
        int? ProductionYear, 
        DiscType DiscType 
        ) : base(Id, Identifier, ProductionCountry, ProductionYear, DiscType)
    {
        SongwriterId = songwriterId;
    }
}
