using MusicManager.Domain.Enums;
using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos
{
    public record CompilationAddDTO(
        SongwriterId SongwriterId,
        string Identifier,
        DiscType DiscType
        );
}