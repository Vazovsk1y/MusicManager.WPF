using MusicManager.Domain.Models;

namespace MusicManager.Services.Contracts.Dtos;

public record SongwriterLookupDTO(
    SongwriterId Id, 
    string FullName
    );
