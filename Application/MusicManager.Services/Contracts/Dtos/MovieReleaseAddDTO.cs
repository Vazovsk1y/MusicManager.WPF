using MusicManager.Domain.Enums;
using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos
{
    public record MovieReleaseAddDTO(
        IEnumerable<MovieId> MoviesLinks,
        string Identifier,
        DiscType DiscType
        );
}