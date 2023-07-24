using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;

namespace MusicManager.Services.Contracts.Dtos
{
    public record MovieReleaseDTO : DiscDTO
    {
        public MovieReleaseDTO(
            DiscId Id, 
            string Identifier, 
            string ProductionCountry, 
            string ProductionYear, 
            DiscType DiscType, 
            IEnumerable<SongDTO> SongDTOs) : base(Id, Identifier, ProductionCountry, ProductionYear, DiscType, SongDTOs)
        {
        }
    }
}