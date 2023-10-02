using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos
{
    public record CompilationAddDTO(
        SongwriterId SongwriterId,
        string Identifier,
        DiscType DiscType,
        int? ProductionYear,
        string? ProductionCountry
        );
}