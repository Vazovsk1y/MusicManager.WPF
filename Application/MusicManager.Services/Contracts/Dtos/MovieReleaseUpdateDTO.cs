using MusicManager.Domain.Common;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos;

public record MovieReleaseUpdateDTO(
        DiscId Id,
        string Identifier,
        string? ProductionCountry,
        int? ProductionYear,
        DiscType DiscType);