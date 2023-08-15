using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos
{
    public record MovieReleaseDTO : DiscDTO
    {
        public IEnumerable<MovieId> MoviesLinks { get; }

        public MovieReleaseDTO(
            DiscId Id,
            IEnumerable<MovieId> moviesLinks,
            string Identifier, 
            string? ProductionCountry, 
            int? ProductionYear, 
            DiscType DiscType, 
            IEnumerable<SongDTO> SongDTOs) : base(Id, Identifier, ProductionCountry, ProductionYear, DiscType, SongDTOs)
        {
            MoviesLinks = moviesLinks;
        }
    }
}