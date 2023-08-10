using MusicManager.Domain.Common;

namespace MusicManager.Services.Contracts.Dtos
{
    public record SongAddDTO(
        DiscId DiscId,
        string PlaybackFilePath
        );

}