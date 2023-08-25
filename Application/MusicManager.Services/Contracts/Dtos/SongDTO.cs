using MusicManager.Domain.Common;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos;

public record SongDTO(
    SongId Id, 
    DiscId DiscId,
    string Name,
    int SongNumber,
    SongFileType ExecutableType,
    string? DiscNumber,
    TimeSpan Duration
    );
