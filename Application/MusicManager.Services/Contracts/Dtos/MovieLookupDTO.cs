using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos;

public record MovieLookupDTO(
    MovieId MovieId,
    string Title,
    int ProductionYear
    );

