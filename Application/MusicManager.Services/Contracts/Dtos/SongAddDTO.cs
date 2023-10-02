using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos
{
    public record SongAddDTO(
        DiscId DiscId,
        SongFile SongFile,
        DiscNumber? DiscNumber = null
        );

}