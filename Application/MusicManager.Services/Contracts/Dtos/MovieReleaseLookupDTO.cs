using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos;

public record MovieReleaseLookupDTO(
    DiscId Id,
    string Identifier,
    DiscType DiscType
    );