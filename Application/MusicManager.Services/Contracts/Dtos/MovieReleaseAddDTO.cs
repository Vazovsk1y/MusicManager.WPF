using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Contracts.Dtos;

public record MovieLinkDTO(MovieId MovieId, bool AddReleaseAsFolder = true);

public record MovieReleaseAddDTO(
    IEnumerable<MovieLinkDTO> AssociatedMoviesLinks,
    string Identifier,
    DiscType DiscType,
    int? ProductionYear,
    string? ProductionCountry
    );
