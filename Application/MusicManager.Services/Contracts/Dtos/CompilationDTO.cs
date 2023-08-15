using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos
{
    public record CompilationDTO : DiscDTO
    {
        public SongwriterId SongwriterId { get; }

        public CompilationDTO(
            DiscId Id,
            SongwriterId songwriterId,
            string Identifier, 
            string? ProductionCountry, 
            int? ProductionYear, 
            DiscType DiscType, 
            IEnumerable<SongDTO> SongDTOs
            ) : base(Id, Identifier, ProductionCountry, ProductionYear, DiscType, SongDTOs)
        {
            SongwriterId = songwriterId;
        }
    }
}