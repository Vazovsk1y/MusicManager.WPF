using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos;

public record SongwriterDTO(
    SongwriterId Id,
    string Name,
    string LastName);