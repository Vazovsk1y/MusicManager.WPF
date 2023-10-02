using MusicManager.Domain.Common;

namespace MusicManager.Services.Contracts.Dtos
{
    public record SongsAddFromCueDTO(
        DiscId DiscId,
        string CueFilePath
        );

}