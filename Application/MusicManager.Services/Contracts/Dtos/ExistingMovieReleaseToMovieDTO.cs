using MusicManager.Domain.Common;
using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos;

public record ExistingMovieReleaseToMovieDTO(
    MovieId MovieId,
    DiscId DiscId);


