using MusicManager.Domain.Enums;
using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos
{
    public record MovieReleaseAddDTO(
        MovieId MovieId,
        string Identifier,
        DiscType DiscType
        );
}