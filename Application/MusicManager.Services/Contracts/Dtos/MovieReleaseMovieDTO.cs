using MusicManager.Domain.Common;
using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos;

public record MovieReleaseMovieDTO(
    MovieId MovieId,
    DiscId DiscId);


