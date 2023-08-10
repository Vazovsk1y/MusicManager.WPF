using MusicManager.Domain.Common;

namespace MusicManager.Services.Contracts.Dtos
{
    public record DiscLookupDTO(
        DiscId DiscId,
        string Identificator
        );
}