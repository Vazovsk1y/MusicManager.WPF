using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos;

public record SongUpdateDTO(
    SongId SongId,
    string Name,
    int SongOrder
    );