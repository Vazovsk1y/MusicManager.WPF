using MusicManager.Domain.Common;
using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos
{
    public record SongDTO(
        SongId Id, 
        DiscId DiscId,
        string Name,
        string? DiscNumber,
        TimeSpan? Duration
        );
}