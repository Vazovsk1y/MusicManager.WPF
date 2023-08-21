using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos;

public record MovieUpdateDTO(
    MovieId Id,
    string Title,
    string ProductionCountry,
    int? ProductionYear,
    string DirectorName,
    string DirectorLastName);